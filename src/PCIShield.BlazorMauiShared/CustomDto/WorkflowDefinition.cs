using MudBlazor;
namespace PCIShield.Domain.ModelsDto;
public enum MerchantStatus
{
    Draft = 0,
    InReview = 1,
    Approved = 2,
    Rejected = 3,
    Suspended = 4,
    Active = 5,
    Inactive = 6,
    Deleted = 7
}
public enum SupplierStatus
{
    Draft = 0,
    InReview = 1,
    Approved = 2,
    Rejected = 3,
    Suspended = 4,
    Active = 5,
    Inactive = 6,
    Deleted = 7
}
public enum PurchaseStatus
{
    Draft = 0,
    InReview = 1,
    Approved = 2,
    Rejected = 3,
    Suspended = 4,
    Active = 5,
    Inactive = 6,
    Deleted = 7
}
public enum InvoiceStatus
{
    Draft = 0,
    InReview = 1,
    Approved = 2,
    Rejected = 3,
    Suspended = 4,
    Active = 5,
    Inactive = 6,
    Deleted = 7
}
public class WorkflowState
{
    public string Name { get; set; }
    public HashSet<string> AllowedTransitions { get; set; }
    public List<string> RequiredFields { get; set; }
    public Color StateColor { get; set; }
    public string Icon { get; set; }
    public string LocalizationKey { get; set; }
    public Dictionary<string, Func<object, bool>> StateValidators { get; set; }
    public WorkflowState(
        string name,
        IEnumerable<string> allowedTransitions,
        IEnumerable<string> requiredFields,
        Color stateColor,
        string icon)
    {
        Name = name;
        AllowedTransitions = new HashSet<string>(allowedTransitions);
        RequiredFields = new List<string>(requiredFields);
        StateColor = stateColor;
        Icon = icon;
        LocalizationKey = $"State_{name}";
        StateValidators = new Dictionary<string, Func<object, bool>>();
    }
}
public class WorkflowDefinition
{
    private readonly Dictionary<string, HashSet<string>> _requiredApprovals = new();
    private readonly Dictionary<string, List<string>> _stateSequences = new();
    private readonly Dictionary<string, Func<object, bool>> _businessRules;
    private readonly Dictionary<string, WorkflowState> _states = new();
    private void InitializeWorkflowStates()
    {
        AddState(
            MerchantStatus.Draft.ToString(),
            new[] { MerchantStatus.InReview.ToString() },
            new[] { "MerchantCode", "MerchantFirstName" },
            Color.Default,
            Icons.Material.Filled.Edit
        );
        AddState(
            MerchantStatus.InReview.ToString(),
            new[] {
                MerchantStatus.Approved.ToString(),
                MerchantStatus.Rejected.ToString()
            },
            new[] { "MerchantCreditTermDays", "MerchantCreditLimitAmount" },
            Color.Info,
            Icons.Material.Filled.Preview
        );
        AddState(
            MerchantStatus.Approved.ToString(),
            new[] { MerchantStatus.Active.ToString() },
            new[] { "CreatedDate", "CreatedBy" },
            Color.Success,
            Icons.Material.Filled.CheckCircle
        );
        AddState(
            MerchantStatus.Rejected.ToString(),
            new[] { MerchantStatus.Draft.ToString() },
            Array.Empty<string>(),
            Color.Error,
            Icons.Material.Filled.Cancel
        );
        AddState(
            MerchantStatus.Active.ToString(),
            new[] { MerchantStatus.Inactive.ToString() },
            new[] { "IsActive" },
            Color.Success,
            Icons.Material.Filled.PlayCircle
        );
        AddState(
            MerchantStatus.Inactive.ToString(),
            new[] { MerchantStatus.Active.ToString() },
            Array.Empty<string>(),
            Color.Warning,
            Icons.Material.Filled.PauseCircle
        );
        AddState(
            MerchantStatus.Deleted.ToString(),
            Array.Empty<string>(),
            Array.Empty<string>(),
            Color.Error,
            Icons.Material.Filled.Delete
        );
        AddRequiredApprovals(MerchantStatus.Approved.ToString(), new[] { "Reviewer", "Manager" });
        AddRequiredApprovals(MerchantStatus.Active.ToString(), new[] { "Manager" });
    }
    public WorkflowDefinition()
    {
        _businessRules = InitializeBusinessRules();
        InitializeWorkflowStates();
    }
    private Dictionary<string, Func<object, bool>> InitializeBusinessRules()
    {
        return new Dictionary<string, Func<object, bool>>
        {
            {"ValidateDocumentContent", model => HasRequiredDocumentFields(model)},
            {"ValidateOrderAmount", model => ValidateOrderTotals(model)},
            {"ValidatePaymentAmount", model => ValidatePaymentDetails(model)},
            {"ValidateActiveState", model => ValidateActiveStateRequirements(model)}
        };
    }
    public void AddState(string stateName, string[] allowedTransitions, string[] requiredFields = null,
        Color stateColor = default, string icon = null)
    {
        if (string.IsNullOrWhiteSpace(stateName)) return;
        var newState = new WorkflowState(
            stateName,
            allowedTransitions ?? Array.Empty<string>(),
            requiredFields ?? Array.Empty<string>(),
            stateColor,
            icon ?? Icons.Material.Filled.Circle
        );
        _states[stateName] = newState;
    }
    public bool HasState(string stateName)
    {
        return _states.ContainsKey(stateName);
    }
    public bool ValidateStateConfiguration(string stateName)
    {
        if (!_states.TryGetValue(stateName, out var state))
            return false;
        return state.AllowedTransitions != null &&
               state.RequiredFields != null &&
               !string.IsNullOrWhiteSpace(state.LocalizationKey) &&
               state.AllowedTransitions.All(t => !string.IsNullOrWhiteSpace(t)) &&
               state.RequiredFields.All(f => !string.IsNullOrWhiteSpace(f));
    }
    public void AddRequiredApprovals(string stateName, string[] roles)
    {
        if (string.IsNullOrWhiteSpace(stateName))
            throw new ArgumentException("State name cannot be empty", nameof(stateName));
        if (roles == null || roles.Length == 0)
            throw new ArgumentException("At least one role must be specified", nameof(roles));
        _requiredApprovals[stateName] = new HashSet<string>(roles);
    }
    public void AddStateSequence(string sequenceName, string[] states)
    {
        if (string.IsNullOrWhiteSpace(sequenceName))
            throw new ArgumentException("Sequence name cannot be empty", nameof(sequenceName));
        if (states == null || states.Length == 0)
            throw new ArgumentException("At least one state must be specified", nameof(states));
        foreach (var state in states)
        {
            if (!_states.ContainsKey(state))
                throw new InvalidOperationException($"State '{state}' does not exist");
        }
        _stateSequences[sequenceName] = new List<string>(states);
    }
    public bool CanTransition(string fromState, string toState)
    {
        return _states.TryGetValue(fromState, out var state) &&
               state.AllowedTransitions.Contains(toState);
    }
    public bool IsValidTransition(string currentState, string newState)
    {
        return _states.ContainsKey(currentState) &&
               _states[currentState].AllowedTransitions.Contains(newState);
    }
    public bool IsValidSequence(string processType, string currentState, string newState)
    {
        if (!_stateSequences.ContainsKey(processType)) return true;
        var sequence = _stateSequences[processType];
        var currentIndex = sequence.IndexOf(currentState);
        var newIndex = sequence.IndexOf(newState);
        return currentIndex != -1 && newIndex != -1 && newIndex > currentIndex;
    }
    public bool HasRequiredFields(string state, object model)
    {
        if (!_states.TryGetValue(state, out var workflowState)) return true;
        var properties = model.GetType().GetProperties();
        return workflowState.RequiredFields.All(field =>
            properties.Any(p => p.Name == field && p.GetValue(model) != null));
    }
    public bool HasRequiredApprovals(string state, HashSet<string> providedApprovals)
    {
        return !_requiredApprovals.ContainsKey(state) ||
               _requiredApprovals[state].All(role => providedApprovals.Contains(role));
    }
    public Color GetStateColor(string state)
    {
        return _states.TryGetValue(state, out var workflowState)
            ? workflowState.StateColor
            : Color.Default;
    }
    public string GetStateIcon(string state)
    {
        return _states.TryGetValue(state, out var workflowState)
            ? workflowState.Icon
            : Icons.Material.Filled.Circle;
    }
    public List<(string State, bool IsAllowed)> GetAvailableTransitions(string currentState)
    {
        if (!_states.TryGetValue(currentState, out var state))
            return new List<(string, bool)>();
        return _states.Keys
            .Select(s => (State: s, IsAllowed: state.AllowedTransitions.Contains(s)))
            .ToList();
    }
    public IEnumerable<string> GetRequiredFields(string state)
    {
        return _states.TryGetValue(state, out var workflowState)
            ? workflowState.RequiredFields
            : Array.Empty<string>();
    }
    public IEnumerable<string> GetRequiredApprovals(string state)
    {
        return _requiredApprovals.TryGetValue(state, out var approvals)
            ? approvals
            : Array.Empty<string>();
    }
    public WorkflowState GetState(string stateName)
    {
        return _states.TryGetValue(stateName, out var state)
            ? state
            : null;
    }
    public IEnumerable<string> GetSequenceStates(string sequenceName)
    {
        return _stateSequences.TryGetValue(sequenceName, out var sequence)
            ? sequence
            : Array.Empty<string>();
    }
    private bool HasRequiredDocumentFields(object model)
    {
        var properties = model.GetType().GetProperties();
        var requiredFields = new[] { "Title", "Content", "Version" };
        return requiredFields.All(field =>
            properties.Any(p => p.Name == field && p.GetValue(model) != null));
    }
    private bool ValidateOrderTotals(object model) => true;
    private bool ValidatePaymentDetails(object model) => true;
    private bool ValidateActiveStateRequirements(object model)
    {
        var properties = model.GetType().GetProperties();
        var requiredActiveFields = new[] { "Name", "Description", "Status" };
        var hasRequiredFields = requiredActiveFields.All(field =>
            properties.Any(p => p.Name == field && p.GetValue(model) != null));
        var statusProp = properties.FirstOrDefault(p => p.Name == "Status");
        var status = statusProp?.GetValue(model)?.ToString();
        var validActiveStatuses = new[] { "Active", "Pending", "InProcess" };
        return hasRequiredFields && status != null && validActiveStatuses.Contains(status);
    }
}
public class ApprovalDto
{
    public Guid EntityId { get; set; }
    public string Comment { get; set; }
    public int State { get; set; }
    public DateTime ApprovalDate { get; set; }
    public string ApproverRole { get; set; }
}