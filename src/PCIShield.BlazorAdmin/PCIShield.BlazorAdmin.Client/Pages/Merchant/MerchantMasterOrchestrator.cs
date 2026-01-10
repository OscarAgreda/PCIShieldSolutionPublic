using System.Net.Mail;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;

using BlazorMauiShared.Models.Control;
using BlazorMauiShared.Models.Control;
using BlazorMauiShared.Models.Merchant;

using FluentValidation;
using FluentValidation.Results;
using PCIShield.BlazorAdmin.Client.Shared.Validation;
using PCIShield.BlazorAdmin.Client.SignalR;
using PCIShield.BlazorMauiShared.Models.Merchant;
using PCIShield.Client.Services.Merchant;
using PCIShield.Domain.ModelsDto;

using LanguageExt;
using LanguageExt.Common;

using static LanguageExt.Prelude;

using Array = System.Array;
using Unit = System.Reactive.Unit;

namespace PCIShield.BlazorAdmin.Client.Pages.Merchant;

public class MerchantMasterOrchestrator : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    private readonly ILogger _logger;

    private readonly SemaphoreSlim _saveSem = new(1, 1);

    private readonly IHttpMerchantClientService _service;

    private readonly ISignalRNotificationStrategy _signalR;

    private readonly UpdateMerchantValidator _validator;

    private bool _isLoading;

    private bool _isNew;
    private ControlDto _lastKnownControl;
    private WorkflowDefinition _workflowDefinition = new();

    public MerchantMasterOrchestrator(
        IHttpMerchantClientService svc,
        ISignalRNotificationStrategy sig,
        ILogger logger,
        UpdateMerchantValidator val)
    {
        _service = svc;
        _signalR = sig;
        _logger = logger;
        _validator = val;
    }

    public MerchantDto Merchant { get; set; } = new();

    private bool _isDisposed;

    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                if (!_isDisposed)
                {
                    try { RefreshRequested.OnNext(Unit.Default); } catch (ObjectDisposedException) { }
                }
            }
        }
    }

    public Subject<Unit> RefreshRequested { get; } = new();
    public ISelectDataProvider<ControlDto, Guid> ControlDataProvider { get; private set; }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propName) =>
    {
        if (model is not MerchantDto dto)
        {
            return Array.Empty<string>();
        }

        ValidationContext<MerchantDto>? ctx =
            ValidationContext<MerchantDto>.CreateWithOptions(dto, x => x.IncludeProperties(propName));
        ValidationResult? result = await _validator.ValidateAsync(ctx);
        if (result.IsValid)
        {
            return Array.Empty<string>();
        }

        return result.Errors.Select(e => e.ErrorMessage);
    };

    public WorkflowDefinition WorkflowDefinition => _workflowDefinition;

    public async Task ApproveAsync(string comment)
    {
        GenericSignalREvent evt = new(
            "Merchant",
            Merchant.MerchantId,
            SignalROperations.Approval,
            "User");
        evt.Parameters["Comment"] = comment;
        await _signalR.SendNotificationAsync(evt);
    }

    public bool CanApprove()
    {
        return true;
    }

    public void Dispose()
    {
        _isDisposed = true;
        try
        {
            _disposables.Dispose();
        }
        catch
        {
        }

        try
        {
            RefreshRequested.Dispose();
        }
        catch
        {
        }
    }

    public List<string> GetPaymentConditions()
    {
        return ["Credit", "Cash"];
    }

    public async Task InitializeAsync(Guid merchantId, string editingUserId)
    {
        await _signalR.EnsureConnectedAsync();
        _signalR.GetEventStream()
            .Where(e => e.AggregateName == "Merchant" && e.EntityId == merchantId)
            .Subscribe(
                evt =>
                {
                    _logger.LogInformation("Orchestrator sees event {Op} for {Cid}", evt.Operation, evt.EntityId);
                })
            .DisposeWith(_disposables);
        if (merchantId == Guid.Empty)
        {
            _isNew = true;
            await InitNewMerchant();
        }
        else
        {
            await NotifyBeingEdited(merchantId, editingUserId);
            await LoadExistingMerchant(merchantId);
        }
        SetupControlDescendantDataProvider();
    }

    public bool IsFieldComplete(string fieldName)
    {
        PropertyInfo? prop = typeof(MerchantDto).GetProperty(fieldName);
        if (prop == null)
        {
            return false;
        }

        object? val = prop.GetValue(Merchant);
        if (val == null)
        {
            return false;
        }

        return val switch
        {
            string str => !string.IsNullOrWhiteSpace(str),
            DateTime dt => dt != default,
            Guid g => g != Guid.Empty,
            bool b => b,
            _ => true
        };
    }

    public async Task NotifyEditingFinished(Guid id, string userId)
    {
        GenericSignalREvent evt = new("Merchant", id, SignalROperations.EditingFinished, userId);
        await _signalR.SendNotificationAsync(evt);
    }

    public Task OnAssessmentChildTabAdded(AssessmentDto cp)
    {
        Merchant.Assessments.Add(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnAssessmentChildTabDeleted(AssessmentDto cp)
    {
        Merchant.Assessments.Remove(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public Task OnAssessmentChildTabUpdated(AssessmentDto cp)
    {
        int i = Merchant.Assessments.FindIndex(x => x.AssessmentId == cp.AssessmentId);
        if (i >= 0)
        {
            Merchant.Assessments[i] = cp;
        }

        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnAssetChildTabAdded(AssetDto cp)
    {
        Merchant.Assets.Add(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnAssetChildTabDeleted(AssetDto cp)
    {
        Merchant.Assets.Remove(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public Task OnAssetChildTabUpdated(AssetDto cp)
    {
        int i = Merchant.Assets.FindIndex(x => x.AssetId == cp.AssetId);
        if (i >= 0)
        {
            Merchant.Assets[i] = cp;
        }

        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnCompensatingControlChildTabAdded(CompensatingControlDto cp)
    {
        Merchant.CompensatingControls.Add(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnCompensatingControlChildTabDeleted(CompensatingControlDto cp)
    {
        Merchant.CompensatingControls.Remove(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public Task OnCompensatingControlChildTabUpdated(CompensatingControlDto cp)
    {
        int i = Merchant.CompensatingControls.FindIndex(x => x.CompensatingControlId == cp.CompensatingControlId);
        if (i >= 0)
        {
            Merchant.CompensatingControls[i] = cp;
        }

        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnComplianceOfficerChildTabAdded(ComplianceOfficerDto cp)
    {
        Merchant.ComplianceOfficers.Add(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnComplianceOfficerChildTabDeleted(ComplianceOfficerDto cp)
    {
        Merchant.ComplianceOfficers.Remove(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public Task OnComplianceOfficerChildTabUpdated(ComplianceOfficerDto cp)
    {
        int i = Merchant.ComplianceOfficers.FindIndex(x => x.ComplianceOfficerId == cp.ComplianceOfficerId);
        if (i >= 0)
        {
            Merchant.ComplianceOfficers[i] = cp;
        }

        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnCryptographicInventoryChildTabAdded(CryptographicInventoryDto cp)
    {
        Merchant.CryptographicInventories.Add(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnCryptographicInventoryChildTabDeleted(CryptographicInventoryDto cp)
    {
        Merchant.CryptographicInventories.Remove(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public Task OnCryptographicInventoryChildTabUpdated(CryptographicInventoryDto cp)
    {
        int i = Merchant.CryptographicInventories.FindIndex(x => x.CryptographicInventoryId == cp.CryptographicInventoryId);
        if (i >= 0)
        {
            Merchant.CryptographicInventories[i] = cp;
        }

        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnEvidenceChildTabAdded(EvidenceDto cp)
    {
        Merchant.Evidences.Add(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnEvidenceChildTabDeleted(EvidenceDto cp)
    {
        Merchant.Evidences.Remove(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public Task OnEvidenceChildTabUpdated(EvidenceDto cp)
    {
        int i = Merchant.Evidences.FindIndex(x => x.EvidenceId == cp.EvidenceId);
        if (i >= 0)
        {
            Merchant.Evidences[i] = cp;
        }

        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnNetworkSegmentationChildTabAdded(NetworkSegmentationDto cp)
    {
        Merchant.NetworkSegmentations.Add(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnNetworkSegmentationChildTabDeleted(NetworkSegmentationDto cp)
    {
        Merchant.NetworkSegmentations.Remove(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public Task OnNetworkSegmentationChildTabUpdated(NetworkSegmentationDto cp)
    {
        int i = Merchant.NetworkSegmentations.FindIndex(x => x.NetworkSegmentationId == cp.NetworkSegmentationId);
        if (i >= 0)
        {
            Merchant.NetworkSegmentations[i] = cp;
        }

        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnPaymentChannelChildTabAdded(PaymentChannelDto cp)
    {
        Merchant.PaymentChannels.Add(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnPaymentChannelChildTabDeleted(PaymentChannelDto cp)
    {
        Merchant.PaymentChannels.Remove(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public Task OnPaymentChannelChildTabUpdated(PaymentChannelDto cp)
    {
        int i = Merchant.PaymentChannels.FindIndex(x => x.PaymentChannelId == cp.PaymentChannelId);
        if (i >= 0)
        {
            Merchant.PaymentChannels[i] = cp;
        }

        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnServiceProviderChildTabAdded(ServiceProviderDto cp)
    {
        Merchant.ServiceProviders.Add(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnServiceProviderChildTabDeleted(ServiceProviderDto cp)
    {
        Merchant.ServiceProviders.Remove(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public Task OnServiceProviderChildTabUpdated(ServiceProviderDto cp)
    {
        int i = Merchant.ServiceProviders.FindIndex(x => x.ServiceProviderId == cp.ServiceProviderId);
        if (i >= 0)
        {
            Merchant.ServiceProviders[i] = cp;
        }

        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public void UpdateControl(ControlDto selected)
    {
        if (selected != null)
        {
            _lastKnownControl = selected;
            Merchant.ControlId = selected.ControlId;
            Merchant.Control = selected;
            RefreshRequested.OnNext(Unit.Default);
        }
    }

    private void SetupControlDescendantDataProvider()
    {
        ControlDataProvider = new EnhancedSelectDataProvider<ControlDto, Guid>(
            Merchant?.Control,
            async (search, pageNumber, pageSize) =>
            {
                try
                {
                    if (pageNumber == 1 && _lastKnownControl != null)
                    {
                        if (!string.IsNullOrEmpty(search) &&
                            GetControlDisplay(_lastKnownControl).Contains(
                                search,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            return (new[] { _lastKnownControl }, true);
                        }
                    }

                    ListControlResponse? response = await _service.GetFilteredControlsListAsync(
                        pageNumber,
                        pageSize,
                        string.IsNullOrEmpty(search) ? null : new Dictionary<string, string> { { "search", search } });
                    if (response == null)
                    {
                        return (_lastKnownControl != null
                            ?[_lastKnownControl]
                                : Array.Empty<ControlDto>(), false);
    }

    List<ControlDto> items = response.Controls?.ToList() ?? [];
    bool hasMore = response.Count > pageNumber * pageSize;
                    if (pageNumber == 1 && _lastKnownControl != null)
                    {
                        if (!items.Any(
                                x => x.ControlId == _lastKnownControl.ControlId))
                        {
                            items.Insert(0, _lastKnownControl);
                        }
                    }

                    return (items, hasMore);
                }
                catch (Exception ex)
{
    _logger.LogError(ex, "Error loading Controls");
    return (_lastKnownControl != null
        ?[_lastKnownControl]
                        : Array.Empty<ControlDto>(), false);
}
            },
            x => x.ControlId,
            GetControlDisplay);
    }

    private string GetControlDisplay(ControlDto dto)
{
    return dto == null ? string.Empty : $"{dto.ControlCode}";
}

private async Task<Either<Error, LanguageExt.Unit>> CreateMerchant()
{
    try
    {
        IsLoading = true;
        CreateMerchantRequest req = new() { Merchant = Merchant };
        CreateMerchantResponse createResponse = await _service.CreatePostMerchantAsync(req);
        Merchant = createResponse?.Merchant ?? throw new Exception("CreateMerchantResponse was null");
        GenericSignalREvent evt = new(
            "Merchant",
            Merchant.MerchantId,
            SignalROperations.Create,
            "User");
        await _signalR.SendNotificationAsync(evt);
        return Right<Error, LanguageExt.Unit>(LanguageExt.Unit.Default);
    }
    catch (Exception ex)
    {
        return Left<Error, LanguageExt.Unit>(ex.Message);
    }
    finally
    {
        IsLoading = false;
        RefreshRequested.OnNext(Unit.Default);
    }
}

private void FormatDatesForServer()
{
    if (Merchant == null)
    {
        return;
    }

    if (Merchant.CreatedAt != DateTime.MinValue)
    {
        Merchant.CreatedAt = Merchant.CreatedAt.ToUniversalTime();
    }

    if (Merchant.UpdatedAt != DateTime.MinValue)
    {
        if (Merchant.UpdatedAt.HasValue)
            Merchant.UpdatedAt = Merchant.UpdatedAt.Value.ToUniversalTime();
    }
    if (Merchant.LastAssessmentDate.HasValue && Merchant.LastAssessmentDate != DateTime.MinValue)
    {
        Merchant.LastAssessmentDate = Merchant.LastAssessmentDate.Value.ToUniversalTime();
    }
    if (Merchant.NextAssessmentDue != DateTime.MinValue)
    {
        Merchant.NextAssessmentDue = Merchant.NextAssessmentDue.ToUniversalTime();
    }
}

private void FormatDatesForUi()
{
    if (Merchant == null)
    {
        return;
    }

    if (Merchant.CreatedAt != DateTime.MinValue)
    {
        Merchant.CreatedAt = Merchant.CreatedAt.ToLocalTime();
    }

    if (Merchant.UpdatedAt != DateTime.MinValue)
    {
        if (Merchant.UpdatedAt.HasValue)
            Merchant.UpdatedAt = Merchant.UpdatedAt.Value.ToLocalTime();
    }
    if (Merchant.LastAssessmentDate.HasValue && Merchant.LastAssessmentDate != DateTime.MinValue)
    {
        Merchant.LastAssessmentDate = Merchant.LastAssessmentDate.Value.ToLocalTime();
    }
    if (Merchant.NextAssessmentDue != DateTime.MinValue)
    {
        Merchant.NextAssessmentDue = Merchant.NextAssessmentDue.ToLocalTime();
    }
}
private bool IsValidEmail(string email)
{
    try
    {
        MailAddress mail = new(email);
        return mail.Address == email;
    }
    catch
    {
        return false;
    }
}

private async Task<Either<Error, LanguageExt.Unit>> UpdateMerchant()
{
    try
    {
        IsLoading = true;
        UpdateMerchantRequest req = new() { Merchant = Merchant };
        UpdateMerchantResponse updateResponse = await _service.UpdatePutMerchantAsync(req);
        if (updateResponse?.Merchant is null)
        {
            throw new Exception("UpdateMerchantResponse was null");
        }

        GenericSignalREvent evt = new(
            "Merchant",
            Merchant.MerchantId,
            SignalROperations.Update,
            "User");
        await _signalR.SendNotificationAsync(evt);
        return Right<Error, LanguageExt.Unit>(LanguageExt.Unit.Default);
    }
    catch (Exception ex)
    {
        return Left<Error, LanguageExt.Unit>(ex.Message);
    }
    finally
    {
        IsLoading = false;
        RefreshRequested.OnNext(Unit.Default);
    }
}

private async Task InitNewMerchant()
{
    IsLoading = true;
    try
    {
        Merchant = Merchant.CloneAsNew();
        Merchant.ControlId = Guid.Empty;
        Merchant.Control = new ControlDto();

        FormatDatesForUi();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in Init NewMerchant");
    }
    finally
    {
        IsLoading = false;
        RefreshRequested.OnNext(Unit.Default);
    }
}

public async Task<Either<Error, LanguageExt.Unit>> SaveMerchantAsync()
{
    try
    {
        await _saveSem.WaitAsync();

        var saver = new SaveEntityStrategy<MerchantDto>(
              validator: _validator,
              create: (_, ct) => CreateMerchant(),
              update: (_, ct) => UpdateMerchant(),
              isNew: m => m.MerchantId == Guid.Empty,
              preSave: _ => FormatDatesForServer()
          );

        Either<Error, LanguageExt.Unit> result = await saver.SaveAsync(Merchant);
        result.IfLeft(e => _logger.LogWarning("Merchant save failed: {Error}", e.Message));

        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error saving merchant");
        return Left<Error, LanguageExt.Unit>(ex.Message);
    }
    finally
    {
        _saveSem.Release();
    }
}
private async Task LoadExistingMerchant(Guid id)
{
    IsLoading = true;
    try
    {
        GetByIdMerchantResponse? resp = await _service.GetOneFullMerchantByIdAsync(id);
        if (resp?.Merchant != null)
        {
            Merchant = resp.Merchant;
            if (Merchant.Control != null)
            {
                _lastKnownControl = Merchant.Control;
            }
            FormatDatesForUi();
        }
        else
        {
            Merchant = new MerchantDto();
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error loading existing merchant");
        throw;
    }
    finally
    {
        IsLoading = false;
        if (!_isDisposed)
        {
            try { RefreshRequested.OnNext(Unit.Default); } catch (ObjectDisposedException) { }
        }
    }
}

private async Task NotifyBeingEdited(Guid id, string editingUserId)
{
    GenericSignalREvent evt = new("Merchant", id, SignalROperations.BeingEdited, editingUserId);
    await _signalR.SendNotificationAsync(evt);
}
  }