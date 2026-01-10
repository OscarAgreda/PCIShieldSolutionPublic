namespace PCIShield.BlazorMauiShared
{
    public class StockPredictionDetail
    {
        public decimal AverageDemand { get; set; }
        public decimal PeakDemand { get; set; }
        public decimal StandardDeviation { get; set; }
        public decimal RecommendedReorderPoint { get; set; }
        public decimal RecommendedOrderQuantity { get; set; }
        public decimal StockoutRisk { get; set; }
    }
    public static class StatusValidationHelper
    {
        private static readonly Dictionary<string, string[]> _allowedTransitions = new()
        {
            ["Active"] = new[] { "Inactive", "Deleted" },
            ["Inactive"] = new[] { "Active", "Deleted" },
            ["Deleted"] = Array.Empty<string>(),
            ["Draft"] = new[] { "Pending", "Active", "Voided", "Deleted" },
            ["Pending"] = new[] { "Active", "Voided", "Deleted" },
            ["Voided"] = new[] { "Deleted" },
            ["Approved"] = new[] { "Active", "Voided", "Deleted" },
            ["Rejected"] = new[] { "Draft", "Deleted" },
            ["Hidden"] = new[] { "Visible", "Deleted" },
            ["Visible"] = new[] { "Hidden", "Deleted" },
            ["Foreign"] = new[] { "Local", "Deleted" },
            ["Local"] = new[] { "Foreign", "Deleted" },
            ["Person"] = new[] { "Company", "Deleted" },
            ["Company"] = new[] { "Person", "Deleted" },
            ["Reseller"] = new[] { "Customer", "Deleted" },
            ["Customer"] = new[] { "Reseller", "Deleted" }
        };
        public static bool ValidateStatusChange<T>(T model, object newStatus) where T : class
        {
            if (model is ITrackStatus trackable)
            {
                var currentState = GetCurrentState(trackable, newStatus);
                var targetState = GetTargetState(newStatus);
                return _allowedTransitions.ContainsKey(currentState) &&
                       _allowedTransitions[currentState].Contains(targetState);
            }
            return true;
        }
        private static string GetTargetState(object status) =>
            GetCurrentState(new DefaultTrackStatus(), status);
        private static bool GetBoolValue(object status) =>
            status is bool b && b;
        private static string GetCurrentState(ITrackStatus entity, object status)
        {
            if (entity.IsDeleted) return "Deleted";
            return status.GetType().Name switch
            {
                var s when s.Contains("IsDeleted") => entity.IsDeleted ? "Deleted" : entity.IsActive ? "Active" : "Inactive",
                var s when s.Contains("IsActive") => entity.IsActive ? "Active" : "Inactive",
                var s when s.Contains("IsDraft") => GetBoolValue(status) ? "Draft" : "Active",
                var s when s.Contains("IsVoided") => GetBoolValue(status) ? "Voided" : "Active",
                var s when s.Contains("IsApproved") => GetBoolValue(status) ? "Approved" : "Pending",
                var s when s.Contains("IsRejected") => GetBoolValue(status) ? "Rejected" : "Pending",
                var s when s.Contains("IsPending") => GetBoolValue(status) ? "Pending" : "Active",
                var s when s.Contains("IsHidden") => GetBoolValue(status) ? "Hidden" : "Visible",
                var s when s.Contains("IsForeign") => GetBoolValue(status) ? "Foreign" : "Local",
                var s when s.Contains("IsPerson") => GetBoolValue(status) ? "Person" : "Company",
                var s when s.Contains("IsReseller") => GetBoolValue(status) ? "Reseller" : "Customer",
                var s when s.Contains("IsCustomer") => GetBoolValue(status) ? "Customer" : "Reseller",
                var s when s.Contains("IsElectronic") => GetBoolValue(status) ? "Electronic" : "Physical",
                var s when s.Contains("IsPhysical") => GetBoolValue(status) ? "Physical" : "Electronic",
                var s when s.Contains("IsPrinted") => GetBoolValue(status) ? "Printed" : "Pending",
                var s when s.Contains("IsEmailed") => GetBoolValue(status) ? "Emailed" : "Pending",
                var s when s.Contains("IsPosted") => GetBoolValue(status) ? "Posted" : "Draft",
                var s when s.Contains("IsProcessed") => GetBoolValue(status) ? "Processed" : "Pending",
                var s when s.Contains("IsReconciled") => GetBoolValue(status) ? "Reconciled" : "Pending",
                var s when s.Contains("IsLocked") => GetBoolValue(status) ? "Locked" : "Open",
                _ => entity.IsActive ? "Active" : "Inactive"
            };
        }
        public static bool HasActiveReferences<T>(T model) where T : class
        {
            if (model is IHasActiveReferences hasRefs)
            {
                return hasRefs.GetActiveReferences().Any();
            }
            return false;
        }
        public static bool HasRequiredActiveData<T>(T model) where T : class
        {
            if (model is IRequiresActiveData activeData)
            {
                return activeData.ValidateActiveDataRequirements();
            }
            return true;
        }
        public static bool HasPendingStateValidation<T>(T model) where T : class
        {
            if (model is IValidatePendingState pendingState)
            {
                return pendingState.CanTransitionToPending();
            }
            return true;
        }
        public static bool HasActiveDataValidation<T>(T model) where T : class
        {
            if (model is IValidateDraftState draftState)
            {
                return !draftState.HasActiveData();
            }
            return true;
        }
    }
    public interface IHasActiveReferences
    {
        IEnumerable<string> GetActiveReferences();
    }
    public interface IRequiresActiveData
    {
        bool ValidateActiveDataRequirements();
    }
    public interface IValidatePendingState
    {
        bool CanTransitionToPending();
    }
    public interface IValidateDraftState
    {
        bool HasActiveData();
    }
    public interface ITrackStatus
    {
        bool IsActive { get; }
        bool IsDeleted { get; }
        string Status { get; }
    }
    internal class DefaultTrackStatus : ITrackStatus
    {
        public bool IsActive => true;
        public bool IsDeleted => false;
        public string Status => "Active";
    }
    public interface IStatusAuditable : ITrackStatus
    {
        DateTime CreatedDate { get; }
        string CreatedBy { get; }
        DateTime? LastModifiedDate { get; }
        string LastModifiedBy { get; }
    }
}
