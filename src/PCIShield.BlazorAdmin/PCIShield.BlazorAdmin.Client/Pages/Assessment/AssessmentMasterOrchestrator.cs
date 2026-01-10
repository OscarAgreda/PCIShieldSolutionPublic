using System.Net.Mail;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;

using BlazorMauiShared.Models.ScanSchedule;
using BlazorMauiShared.Models.Vulnerability;
using BlazorMauiShared.Models.PaymentPage;
using BlazorMauiShared.Models.Script;
using BlazorMauiShared.Models.Merchant;
using BlazorMauiShared.Models.Merchant;
using BlazorMauiShared.Models.Assessment;

using FluentValidation;
using FluentValidation.Results;
using PCIShield.BlazorAdmin.Client.Shared.Validation;
using PCIShield.BlazorAdmin.Client.SignalR;
using PCIShield.BlazorMauiShared.Models.Assessment;
using PCIShield.Client.Services.Assessment;
using PCIShield.Domain.ModelsDto;

using LanguageExt;
using LanguageExt.Common;

using static LanguageExt.Prelude;

using Array = System.Array;
using Unit = System.Reactive.Unit;

namespace PCIShield.BlazorAdmin.Client.Pages.Assessment;

public class AssessmentMasterOrchestrator : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    private readonly ILogger _logger;

    private readonly SemaphoreSlim _saveSem = new(1, 1);

    private readonly IHttpAssessmentClientService _service;

    private readonly ISignalRNotificationStrategy _signalR;

    private readonly UpdateAssessmentValidator _validator;

    private bool _isLoading;

    private bool _isNew;

    private ScanScheduleDto _lastKnownScanSchedule;
    private VulnerabilityDto _lastKnownVulnerability;
    private PaymentPageDto _lastKnownPaymentPage;
    private ScriptDto _lastKnownScript;
    private MerchantDto _lastKnownMerchant;
    private WorkflowDefinition _workflowDefinition = new();

    public AssessmentMasterOrchestrator(
        IHttpAssessmentClientService svc,
        ISignalRNotificationStrategy sig,
        ILogger logger,
        UpdateAssessmentValidator val)
    {
        _service = svc;
        _signalR = sig;
        _logger = logger;
        _validator = val;
    }

    public AssessmentDto Assessment { get; set; } = new();

    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                RefreshRequested.OnNext(Unit.Default);
            }
        }
    }

    public Subject<Unit> RefreshRequested { get; } = new();
    public ISelectDataProvider<MerchantDto, Guid> MerchantDataProvider { get; private set; }
    public ISelectDataProvider<ScanScheduleDto, Guid> ScanScheduleDataProvider { get; private set; }
    public ISelectDataProvider<VulnerabilityDto, Guid> VulnerabilityDataProvider { get; private set; }
    public ISelectDataProvider<PaymentPageDto, Guid> PaymentPageDataProvider { get; private set; }
    public ISelectDataProvider<ScriptDto, Guid> ScriptDataProvider { get; private set; }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propName) =>
    {
        if (model is not AssessmentDto dto)
        {
            return Array.Empty<string>();
        }

        ValidationContext<AssessmentDto>? ctx =
            ValidationContext<AssessmentDto>.CreateWithOptions(dto, x => x.IncludeProperties(propName));
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
            "Assessment",
            Assessment.AssessmentId,
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

    public async Task InitializeAsync(Guid assessmentId, string editingUserId)
    {
        await _signalR.EnsureConnectedAsync();
        _signalR.GetEventStream()
            .Where(e => e.AggregateName == "Assessment" && e.EntityId == assessmentId)
            .Subscribe(
                evt =>
                {
                    _logger.LogInformation("Orchestrator sees event {Op} for {Cid}", evt.Operation, evt.EntityId);
                })
            .DisposeWith(_disposables);
        if (assessmentId == Guid.Empty)
        {
            _isNew = true;
            await InitNewAssessment();
        }
        else
        {
            await NotifyBeingEdited(assessmentId, editingUserId);
            await LoadExistingAssessment(assessmentId);
        }
        SetupMerchantParentDataProvider();
        SetupScanScheduleCascadeDataProvider();
        SetupVulnerabilityCascadeDataProvider();
        SetupPaymentPageCascadeDataProvider();
        SetupScriptCascadeDataProvider();
    }

    public bool IsFieldComplete(string fieldName)
    {
        PropertyInfo? prop = typeof(AssessmentDto).GetProperty(fieldName);
        if (prop == null)
        {
            return false;
        }

        object? val = prop.GetValue(Assessment);
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
        GenericSignalREvent evt = new("Assessment", id, SignalROperations.EditingFinished, userId);
        await _signalR.SendNotificationAsync(evt);
    }
    public Task OnControlJoinTabAdded(AssessmentControlDto control)
    {
        Assessment.AssessmentControls.Add(control);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public Task OnControlJoinTabDeleted(AssessmentControlDto control)
    {
        Assessment.AssessmentControls.Remove(control);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public Task OnControlJoinTabUpdated(AssessmentControlDto control)
    {
        int i = Assessment.AssessmentControls.FindIndex(x => x.ControlId == control.ControlId);
        if (i >= 0)
        {
            Assessment.AssessmentControls[i] = control;
        }

        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public Task OnEvidenceJoinTabAdded(ControlEvidenceDto evidence)
    {
        Assessment.ControlEvidences.Add(evidence);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public Task OnEvidenceJoinTabDeleted(ControlEvidenceDto evidence)
    {
        Assessment.ControlEvidences.Remove(evidence);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public Task OnEvidenceJoinTabUpdated(ControlEvidenceDto evidence)
    {
        int i = Assessment.ControlEvidences.FindIndex(x => x.EvidenceId == evidence.EvidenceId);
        if (i >= 0)
        {
            Assessment.ControlEvidences[i] = evidence;
        }

        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public Task OnROCPackageChildTabAdded(ROCPackageDto cp)
    {
        Assessment.ROCPackages.Add(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public Task OnROCPackageChildTabDeleted(ROCPackageDto cp)
    {
        Assessment.ROCPackages.Remove(cp);
        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }

    public Task OnROCPackageChildTabUpdated(ROCPackageDto cp)
    {
        int i = Assessment.ROCPackages.FindIndex(x => x.ROCPackageId == cp.ROCPackageId);
        if (i >= 0)
        {
            Assessment.ROCPackages[i] = cp;
        }

        RefreshRequested.OnNext(Unit.Default);
        return Task.CompletedTask;
    }
    public void UpdateMerchant(MerchantDto selected)
    {
        if (selected != null)
        {
            _lastKnownMerchant = selected;
            Assessment.MerchantId = selected.MerchantId;
            Assessment.Merchant = selected;
            RefreshRequested.OnNext(Unit.Default);
        }
    }

    private void SetupMerchantParentDataProvider()
    {
        MerchantDataProvider = new EnhancedSelectDataProvider<MerchantDto, Guid>(
            Assessment?.Merchant,
            async (search, pageNumber, pageSize) =>
            {
                try
                {
                    if (pageNumber == 1 && _lastKnownMerchant != null)
                    {
                        if (!string.IsNullOrEmpty(search) &&
                            GetMerchantDisplay(_lastKnownMerchant).Contains(
                                search,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            return (new[] { _lastKnownMerchant }, true);
                        }
                    }

                    ListMerchantResponse? response = await _service.GetFilteredMerchantsListAsync(
                        pageNumber,
                        pageSize,
                        string.IsNullOrEmpty(search) ? null : new Dictionary<string, string> { { "search", search } });
                    if (response == null)
                    {
                        return (_lastKnownMerchant != null
                            ? [_lastKnownMerchant]
                            : Array.Empty<MerchantDto>(), false);
                    }

                    List<MerchantDto> items = response.Merchants?.ToList() ?? [];
                    bool hasMore = response.Count > pageNumber * pageSize;
                    if (pageNumber == 1 && _lastKnownMerchant != null)
                    {
                        if (!items.Any(
                                x => x.MerchantId == _lastKnownMerchant.MerchantId))
                        {
                            items.Insert(0, _lastKnownMerchant);
                        }
                    }

                    return (items, hasMore);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading Merchants");
                    return (_lastKnownMerchant != null
                        ? [_lastKnownMerchant]
                        : Array.Empty<MerchantDto>(), false);
                }
            },
            x => x.MerchantId,
            GetMerchantDisplay);
    }

    private string GetMerchantDisplay(MerchantDto dto)
    {
        return dto == null ? string.Empty : $"{dto.MerchantCode}";
    }
    public void UpdateCascadeScanSchedule(ScanScheduleDto selected)
    {
        if (selected == null)
        {
            return;
        }

        _lastKnownScanSchedule = selected;
        Assessment.ScanScheduleId = selected.ScanScheduleId;
        Assessment.ScanSchedule = selected;
        RefreshRequested.OnNext(Unit.Default);
    }

    private string GetScanScheduleDisplay(ScanScheduleDto dto)
    {
        return dto == null ? string.Empty : $"{dto.Frequency}";
    }

    private void SetupScanScheduleCascadeDataProvider()
    {
        ScanScheduleDataProvider = SelectDataProviderHelper.CreatePagedProvider<ScanScheduleDto>(
            async (search, pageNumber, pageSize) =>
            {
                try
                {
                    if (pageNumber == 1 && _lastKnownScanSchedule != null)
                    {
                        if (!string.IsNullOrEmpty(search) &&
                            GetScanScheduleDisplay(_lastKnownScanSchedule).Contains(search, StringComparison.OrdinalIgnoreCase))
                        {
                            return (new[] { _lastKnownScanSchedule }, 1);
                        }
                    }
                    Guid assetId = Assessment.ScanScheduleId;
                    if (assetId == Guid.Empty)
                    {
                        return (Array.Empty<ScanScheduleDto>(), 0);
                    }
                    ListScanScheduleResponse? response = await _service.GetFilteredScanSchedulesListAsync(
                        assetId,
                        pageNumber,
                        pageSize,
                        string.IsNullOrEmpty(search) ? null : new Dictionary<string, string> { { "search", search } },
                        null);
                    List<ScanScheduleDto> items = response?.ScanSchedules?.ToList() ?? [];
                    int totalCount = response?.Count ?? 0;
                    _logger.LogInformation(
                        "ScanSchedules loaded from API: {count}, Response success: {success}",
                        items.Count,
                        response?.IsSuccess);
                    if (pageNumber == 1 && _lastKnownScanSchedule != null)
                    {
                        if (!items.Any(x => x.ScanScheduleId == _lastKnownScanSchedule.ScanScheduleId))
                        {
                            items.Insert(0, _lastKnownScanSchedule);
                            totalCount++;
                        }
                    }

                    return (items, totalCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading ScanSchedules");
                    return (_lastKnownScanSchedule != null
                            ? [_lastKnownScanSchedule]
                            : Array.Empty<ScanScheduleDto>(),
                        _lastKnownScanSchedule != null ? 1 : 0);
                }
            },
            x => x.ScanScheduleId,
            GetScanScheduleDisplay);
    }
    public void UpdateCascadeVulnerability(VulnerabilityDto selected)
    {
        if (selected == null)
        {
            return;
        }

        _lastKnownVulnerability = selected;
        Assessment.VulnerabilityId = selected.VulnerabilityId;
        Assessment.Vulnerability = selected;
        RefreshRequested.OnNext(Unit.Default);
    }

    private string GetVulnerabilityDisplay(VulnerabilityDto dto)
    {
        return dto == null ? string.Empty : $"{dto.VulnerabilityCode}";
    }

    private void SetupVulnerabilityCascadeDataProvider()
    {
        VulnerabilityDataProvider = SelectDataProviderHelper.CreatePagedProvider<VulnerabilityDto>(
            async (search, pageNumber, pageSize) =>
            {
                try
                {
                    if (pageNumber == 1 && _lastKnownVulnerability != null)
                    {
                        if (!string.IsNullOrEmpty(search) &&
                            GetVulnerabilityDisplay(_lastKnownVulnerability).Contains(search, StringComparison.OrdinalIgnoreCase))
                        {
                            return (new[] { _lastKnownVulnerability }, 1);
                        }
                    }
                    Guid assetId = Assessment.VulnerabilityId;
                    if (assetId == Guid.Empty)
                    {
                        return (Array.Empty<VulnerabilityDto>(), 0);
                    }
                    ListVulnerabilityResponse? response = await _service.GetFilteredVulnerabilitiesListAsync(
                        assetId,
                        pageNumber,
                        pageSize,
                        string.IsNullOrEmpty(search) ? null : new Dictionary<string, string> { { "search", search } },
                        null);
                    List<VulnerabilityDto> items = response?.Vulnerabilities?.ToList() ?? [];
                    int totalCount = response?.Count ?? 0;
                    _logger.LogInformation(
                        "Vulnerabilities loaded from API: {count}, Response success: {success}",
                        items.Count,
                        response?.IsSuccess);
                    if (pageNumber == 1 && _lastKnownVulnerability != null)
                    {
                        if (!items.Any(x => x.VulnerabilityId == _lastKnownVulnerability.VulnerabilityId))
                        {
                            items.Insert(0, _lastKnownVulnerability);
                            totalCount++;
                        }
                    }

                    return (items, totalCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading Vulnerabilities");
                    return (_lastKnownVulnerability != null
                            ? [_lastKnownVulnerability]
                            : Array.Empty<VulnerabilityDto>(),
                        _lastKnownVulnerability != null ? 1 : 0);
                }
            },
            x => x.VulnerabilityId,
            GetVulnerabilityDisplay);
    }
    public void UpdateCascadePaymentPage(PaymentPageDto selected)
    {
        if (selected == null)
        {
            return;
        }

        _lastKnownPaymentPage = selected;
        Assessment.PaymentPageId = selected.PaymentPageId;
        Assessment.PaymentPage = selected;

        if (Assessment.Script?.PaymentPageId != selected.PaymentPageId)
        {
          Assessment.Script  = null;
          Assessment.ScriptId  = Guid.Empty;
        }

        RefreshRequested.OnNext(Unit.Default);
    }

    private string GetPaymentPageDisplay(PaymentPageDto dto)
    {
        return dto == null ? string.Empty : $"{dto.ScriptIntegrityHash}";
    }

    private void SetupPaymentPageCascadeDataProvider()
    {
        PaymentPageDataProvider = SelectDataProviderHelper.CreatePagedProvider<PaymentPageDto>(
            async (search, pageNumber, pageSize) =>
            {
                try
                {
                    if (pageNumber == 1 && _lastKnownPaymentPage != null)
                    {
                        if (!string.IsNullOrEmpty(search) &&
                            GetPaymentPageDisplay(_lastKnownPaymentPage).Contains(search, StringComparison.OrdinalIgnoreCase))
                        {
                            return (new[] { _lastKnownPaymentPage }, 1);
                        }
                    }
                    Guid paymentChannelId = Assessment.PaymentPageId;
                    if (paymentChannelId == Guid.Empty)
                    {
                        return (Array.Empty<PaymentPageDto>(), 0);
                    }
                    ListPaymentPageResponse? response = await _service.GetFilteredPaymentPagesListAsync(
                        paymentChannelId,
                        pageNumber,
                        pageSize,
                        string.IsNullOrEmpty(search) ? null : new Dictionary<string, string> { { "search", search } },
                        null);
                    List<PaymentPageDto> items = response?.PaymentPages?.ToList() ?? [];
                    int totalCount = response?.Count ?? 0;
                    _logger.LogInformation(
                        "PaymentPages loaded from API: {count}, Response success: {success}",
                        items.Count,
                        response?.IsSuccess);
                    if (pageNumber == 1 && _lastKnownPaymentPage != null)
                    {
                        if (!items.Any(x => x.PaymentPageId == _lastKnownPaymentPage.PaymentPageId))
                        {
                            items.Insert(0, _lastKnownPaymentPage);
                            totalCount++;
                        }
                    }

                    return (items, totalCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading PaymentPages");
                    return (_lastKnownPaymentPage != null
                            ? [_lastKnownPaymentPage]
                            : Array.Empty<PaymentPageDto>(),
                        _lastKnownPaymentPage != null ? 1 : 0);
                }
            },
            x => x.PaymentPageId,
            GetPaymentPageDisplay);
    }
    public void UpdateCascadeScript(ScriptDto selected)
    {
        if (selected == null)
        {
            return;
        }

        _lastKnownScript = selected;
        Assessment.ScriptId = selected.ScriptId;
        Assessment.Script = selected;
        RefreshRequested.OnNext(Unit.Default);
    }

    private string GetScriptDisplay(ScriptDto dto)
    {
        return dto == null ? string.Empty : $"{dto.ScriptHash}";
    }

    private void SetupScriptCascadeDataProvider()
    {
        ScriptDataProvider = SelectDataProviderHelper.CreatePagedProvider<ScriptDto>(
            async (search, pageNumber, pageSize) =>
            {
                try
                {
                    if (pageNumber == 1 && _lastKnownScript != null)
                    {
                        if (!string.IsNullOrEmpty(search) &&
                            GetScriptDisplay(_lastKnownScript).Contains(search, StringComparison.OrdinalIgnoreCase))
                        {
                            return (new[] { _lastKnownScript }, 1);
                        }
                    }
                    Guid paymentPageId = Assessment.ScriptId;
                    if (paymentPageId == Guid.Empty)
                    {
                        return (Array.Empty<ScriptDto>(), 0);
                    }
                    ListScriptResponse? response = await _service.GetFilteredScriptsListAsync(
                        paymentPageId,
                        pageNumber,
                        pageSize,
                        string.IsNullOrEmpty(search) ? null : new Dictionary<string, string> { { "search", search } },
                        null);
                    List<ScriptDto> items = response?.Scripts?.ToList() ?? [];
                    int totalCount = response?.Count ?? 0;
                    _logger.LogInformation(
                        "Scripts loaded from API: {count}, Response success: {success}",
                        items.Count,
                        response?.IsSuccess);
                    if (pageNumber == 1 && _lastKnownScript != null)
                    {
                        if (!items.Any(x => x.ScriptId == _lastKnownScript.ScriptId))
                        {
                            items.Insert(0, _lastKnownScript);
                            totalCount++;
                        }
                    }

                    return (items, totalCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading Scripts");
                    return (_lastKnownScript != null
                            ? [_lastKnownScript]
                            : Array.Empty<ScriptDto>(),
                        _lastKnownScript != null ? 1 : 0);
                }
            },
            x => x.ScriptId,
            GetScriptDisplay);
    }

    private async Task<Either<Error, LanguageExt.Unit>> CreateAssessment()
    {
        try
        {
            IsLoading = true;
            CreateAssessmentRequest req = new() { Assessment = Assessment };
            CreateAssessmentResponse createResponse = await _service.CreatePostAssessmentAsync(req);
            Assessment = createResponse?.Assessment ?? throw new Exception("CreateAssessmentResponse was null");
            GenericSignalREvent evt = new(
                "Assessment",
                Assessment.AssessmentId,
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
        if (Assessment == null)
        {
            return;
        }

        if (Assessment.CreatedAt != DateTime.MinValue)
        {
            Assessment.CreatedAt = Assessment.CreatedAt.ToUniversalTime();
        }

        if (Assessment.UpdatedAt != DateTime.MinValue)
        {
            if ( Assessment.UpdatedAt.HasValue)
            Assessment.UpdatedAt = Assessment.UpdatedAt.Value.ToUniversalTime();
        }
        if (Assessment.StartDate != DateTime.MinValue)
        {
            Assessment.StartDate = Assessment.StartDate.ToUniversalTime();
        }
        if (Assessment.EndDate != DateTime.MinValue)
        {
            Assessment.EndDate = Assessment.EndDate.ToUniversalTime();
        }
      if (Assessment.CompletionDate.HasValue && Assessment.CompletionDate != DateTime.MinValue)
        {
            Assessment.CompletionDate = Assessment.CompletionDate.Value.ToUniversalTime();
        }
    }

    private void FormatDatesForUi()
    {
        if (Assessment == null)
        {
            return;
        }

        if (Assessment.CreatedAt != DateTime.MinValue)
        {
            Assessment.CreatedAt = Assessment.CreatedAt.ToLocalTime();
        }

        if (Assessment.UpdatedAt != DateTime.MinValue)
        {
            if ( Assessment.UpdatedAt.HasValue)
            Assessment.UpdatedAt = Assessment.UpdatedAt.Value.ToLocalTime();
        }
        if (Assessment.StartDate != DateTime.MinValue)
        {
            Assessment.StartDate = Assessment.StartDate.ToLocalTime();
        }
        if (Assessment.EndDate != DateTime.MinValue)
        {
            Assessment.EndDate = Assessment.EndDate.ToLocalTime();
        }
        if (Assessment.CompletionDate.HasValue && Assessment.CompletionDate != DateTime.MinValue)
        {
            Assessment.CompletionDate = Assessment.CompletionDate.Value.ToLocalTime();
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

    private async Task<Either<Error, LanguageExt.Unit>> UpdateAssessment()
    {
        try
        {
            IsLoading = true;
            UpdateAssessmentRequest req = new() { Assessment = Assessment };
            UpdateAssessmentResponse updateResponse = await _service.UpdatePutAssessmentAsync(req);
            if (updateResponse?.Assessment is null)
            {
                throw new Exception("UpdateAssessmentResponse was null");
            }

            GenericSignalREvent evt = new(
                "Assessment",
                Assessment.AssessmentId,
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

    private async Task InitNewAssessment()
    {
        IsLoading = true;
        try
        {
                Assessment = Assessment.CloneAsNew();
                Assessment.MerchantId = Guid.Empty;
                Assessment.Merchant = new MerchantDto();

            FormatDatesForUi();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Init NewAssessment");
        }
        finally
        {
            IsLoading = false;
            RefreshRequested.OnNext(Unit.Default);
        }
    }

    public async Task<Either<Error, LanguageExt.Unit>> SaveAssessmentAsync()
    {
        try
        {
            await _saveSem.WaitAsync();

            var saver = new SaveEntityStrategy<AssessmentDto>(
                  validator: _validator,
                  create: (_, ct) => CreateAssessment(),
                  update: (_, ct) => UpdateAssessment(),
                  isNew: m => m.AssessmentId == Guid.Empty,
                  preSave: _ => FormatDatesForServer()
              );

            Either<Error, LanguageExt.Unit> result = await saver.SaveAsync(Assessment);
            result.IfLeft(e => _logger.LogWarning("Assessment save failed: {Error}", e.Message));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving assessment");
            return Left<Error, LanguageExt.Unit>(ex.Message);
        }
        finally
        {
            _saveSem.Release();
        }
    }
    private async Task LoadExistingAssessment(Guid id)
    {
        IsLoading = true;
        try
        {
            GetByIdAssessmentResponse? resp = await _service.GetOneFullAssessmentByIdAsync(id);
            if (resp?.Assessment != null)
            {
                Assessment = resp.Assessment;
                if (Assessment.Merchant != null)
                {
                    _lastKnownMerchant = Assessment.Merchant;
                }
                if (Assessment.ScanSchedule != null)
                {
                    _lastKnownScanSchedule = Assessment.ScanSchedule;
                }
                if (Assessment.Vulnerability != null)
                {
                    _lastKnownVulnerability = Assessment.Vulnerability;
                }
                if (Assessment.PaymentPage != null)
                {
                    _lastKnownPaymentPage = Assessment.PaymentPage;
                }
                if (Assessment.Script != null)
                {
                    _lastKnownScript = Assessment.Script;
                }
                FormatDatesForUi();
            }
            else
            {
                Assessment = new AssessmentDto();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading existing assessment");
            throw;
        }
        finally
        {
            IsLoading = false;
            RefreshRequested.OnNext(Unit.Default);
        }
    }

    private async Task NotifyBeingEdited(Guid id, string editingUserId)
    {
        GenericSignalREvent evt = new("Assessment", id, SignalROperations.BeingEdited, editingUserId);
        await _signalR.SendNotificationAsync(evt);
    }
  }