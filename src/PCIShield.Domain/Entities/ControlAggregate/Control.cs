using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Ardalis.GuardClauses;
using System.Collections.Immutable;
using System.Text.Json;
using LanguageExt;
using static LanguageExt.Prelude;
using PCIShield.Domain.Exceptions;
using PCIShield.Domain.Events;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;

namespace PCIShield.Domain.Entities
{
    
    public class Control : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public Guid ControlId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public string ControlCode { get; private set; }
        
        public string RequirementNumber { get; private set; }
        
        public string ControlTitle { get; private set; }
        
        public string ControlDescription { get; private set; }
        
        public string? TestingGuidance { get; private set; }
        
        public int FrequencyDays { get; private set; }
        
        public bool IsMandatory { get; private set; }
        
        public DateTime EffectiveDate { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        
        public Guid CreatedBy { get; private set; }
        
        public DateTime? UpdatedAt { get; private set; }
        
        public Guid? UpdatedBy { get; private set; }
        
        public bool IsDeleted { get; private set; }
        
        public void SetTenantId(Guid tenantId)
        {
            TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
        }
        public void SetControlCode(string controlCode)
        {
            ControlCode = Guard.Against.NullOrEmpty(controlCode, nameof(controlCode));
        }
        public void SetRequirementNumber(string requirementNumber)
        {
            RequirementNumber = Guard.Against.NullOrEmpty(requirementNumber, nameof(requirementNumber));
        }
        public void SetControlTitle(string controlTitle)
        {
            ControlTitle = Guard.Against.NullOrEmpty(controlTitle, nameof(controlTitle));
        }
        public void SetControlDescription(string controlDescription)
        {
            ControlDescription = Guard.Against.NullOrEmpty(controlDescription, nameof(controlDescription));
        }
        public void SetTestingGuidance(string testingGuidance)
        {
            TestingGuidance = testingGuidance;
        }
        public void SetFrequencyDays(int frequencyDays)
        {
            FrequencyDays = Guard.Against.Negative(frequencyDays, nameof(frequencyDays));
        }
        public void SetIsMandatory(bool isMandatory)
        {
            IsMandatory = Guard.Against.Null(isMandatory, nameof(isMandatory));
        }
        public void SetEffectiveDate(DateTime effectiveDate)
        {
            EffectiveDate = Guard.Against.OutOfSQLDateRange(effectiveDate, nameof(effectiveDate));
        }
        public void SetCreatedAt(DateTime createdAt)
        {
            CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
        }
        public void SetCreatedBy(Guid createdBy)
        {
            CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
        }
        public void SetUpdatedAt(DateTime? updatedAt)
        {
            UpdatedAt = updatedAt;
        }
        public void SetUpdatedBy(Guid? updatedBy)
        {
            UpdatedBy = updatedBy;
        }
        public void SetIsDeleted(bool isDeleted)
        {
            IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
        }
        private Control() { }
        
        public Control(Guid controlId, Guid tenantId, string controlCode, string requirementNumber, string controlTitle, string controlDescription, int frequencyDays, bool isMandatory, DateTime effectiveDate, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            this.ControlId = Guard.Against.Default(controlId, nameof(controlId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.ControlCode = Guard.Against.NullOrEmpty(controlCode, nameof(controlCode));
            this.RequirementNumber = Guard.Against.NullOrEmpty(requirementNumber, nameof(requirementNumber));
            this.ControlTitle = Guard.Against.NullOrEmpty(controlTitle, nameof(controlTitle));
            this.ControlDescription = Guard.Against.NullOrEmpty(controlDescription, nameof(controlDescription));
            this.FrequencyDays = Guard.Against.Negative(frequencyDays, nameof(frequencyDays));
            this.IsMandatory = Guard.Against.Null(isMandatory, nameof(isMandatory));
            this.EffectiveDate = Guard.Against.OutOfSQLDateRange(effectiveDate, nameof(effectiveDate));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            
            _assessmentControls = new();
            
            _assetControls = new();
            
            _compensatingControls = new();
            
            _controlEvidences = new();
            
        }
        private readonly List<AssessmentControl> _assessmentControls = new();
        public IEnumerable<AssessmentControl> AssessmentControls => _assessmentControls.AsReadOnly();
        
        private readonly List<AssetControl> _assetControls = new();
        public IEnumerable<AssetControl> AssetControls => _assetControls.AsReadOnly();
        
        private readonly List<CompensatingControl> _compensatingControls = new();
        public IEnumerable<CompensatingControl> CompensatingControls => _compensatingControls.AsReadOnly();
        
        private readonly List<ControlEvidence> _controlEvidences = new();
        public IEnumerable<ControlEvidence> ControlEvidences => _controlEvidences.AsReadOnly();
        
        public override bool Equals(object? obj) =>
        obj is Control control && Equals(control);
        
        public bool Equals(Control other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return ControlId.Equals(other.ControlId);
        }
        
        public override int GetHashCode() => ControlId.GetHashCode();
        
        public static bool operator !=(Control left, Control right) => !(left == right);
        
        public static bool operator ==(Control left, Control right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(ControlCode))
            throw new InvalidOperationException("ControlCode cannot be null or whitespace.");
            if (ControlCode?.Length > 30)
            throw new InvalidOperationException("ControlCode cannot exceed 30 characters.");
            if (string.IsNullOrWhiteSpace(RequirementNumber))
            throw new InvalidOperationException("RequirementNumber cannot be null or whitespace.");
            if (RequirementNumber?.Length > 20)
            throw new InvalidOperationException("RequirementNumber cannot exceed 20 characters.");
            if (string.IsNullOrWhiteSpace(ControlTitle))
            throw new InvalidOperationException("ControlTitle cannot be null or whitespace.");
            if (ControlTitle?.Length > 500)
            throw new InvalidOperationException("ControlTitle cannot exceed 500 characters.");
            if (string.IsNullOrWhiteSpace(ControlDescription))
            throw new InvalidOperationException("ControlDescription cannot be null or whitespace.");
            if (ControlDescription?.Length > -1)
            throw new InvalidOperationException("ControlDescription cannot exceed -1 characters.");
            if (TestingGuidance?.Length > -1)
            throw new InvalidOperationException("TestingGuidance cannot exceed -1 characters.");
            if (EffectiveDate == default)
            throw new InvalidOperationException("EffectiveDate must be set.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
        private void RaiseDomainEvent(BaseDomainEvent domainEvent)
        {
            Events.Add(domainEvent);
        }
        public static Eff<Validation<string, Control>> Create(
        Guid controlId,
        Guid tenantId,
        string controlCode,
        string requirementNumber,
        string controlTitle,
        string controlDescription,
        int frequencyDays,
        bool isMandatory,
        DateTime effectiveDate,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            return Eff(() => CreateControl(
            controlId,
            tenantId,
            controlCode,
            requirementNumber,
            controlTitle,
            controlDescription,
            frequencyDays,
            isMandatory,
            effectiveDate,
            createdAt,
            createdBy,
            isDeleted
            ));
        }
        
        private static Validation<string, Control> CreateControl(
        Guid controlId,
        Guid tenantId,
        string controlCode,
        string requirementNumber,
        string controlTitle,
        string controlDescription,
        int frequencyDays,
        bool isMandatory,
        DateTime effectiveDate,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            try
            {
                var now = DateTime.UtcNow;
                isDeleted = false;
                
                var control = new Control
                ( controlId,
                tenantId,
                controlCode,
                requirementNumber,
                controlTitle,
                controlDescription,
                frequencyDays,
                isMandatory,
                effectiveDate,
                createdAt,
                createdBy,
                isDeleted)
                ;
                
                return Validation<string, Control>.Success(control);
            }
            catch (Exception ex)
            {
                return Validation<string, Control>.Fail(new Seq<string> { ex.Message });
            }
        }
        public static Eff<Validation<string, Control>> Update(
        Control existingControl,
        Guid controlId,
        Guid tenantId,
        string controlCode,
        string requirementNumber,
        string controlTitle,
        string controlDescription,
        string testingGuidance,
        int frequencyDays,
        bool isMandatory,
        DateTime effectiveDate,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            return Eff(() => UpdateControl(
            existingControl,
            controlId,
            tenantId,
            controlCode,
            requirementNumber,
            controlTitle,
            controlDescription,
            testingGuidance,
            frequencyDays,
            isMandatory,
            effectiveDate,
            createdAt,
            createdBy,
            updatedAt.ToOption().ToNullable(),
            updatedBy.ToOption().ToNullable(),
            isDeleted
            ));
        }
        
        private static Validation<string, Control> UpdateControl(
        Control control,
        Guid controlId,
        Guid tenantId,
        string controlCode,
        string requirementNumber,
        string controlTitle,
        string controlDescription,
        string testingGuidance,
        int frequencyDays,
        bool isMandatory,
        DateTime effectiveDate,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            try
            {
                
                control.ControlId = controlId;
                control.TenantId = tenantId;
                control.ControlCode = controlCode;
                control.RequirementNumber = requirementNumber;
                control.ControlTitle = controlTitle;
                control.ControlDescription = controlDescription;
                control.TestingGuidance = testingGuidance;
                control.FrequencyDays = frequencyDays;
                control.IsMandatory = isMandatory;
                control.EffectiveDate = effectiveDate;
                control.CreatedAt = createdAt;
                control.CreatedBy = createdBy;
                control.UpdatedAt = updatedAt;
                control.UpdatedBy = updatedBy;
                control.IsDeleted = isDeleted;
                control.RaiseDomainEvent(new ControlUpdatedEvent(control, "Updated"));
                
                return Success<string, Control>(control);
            }
            catch (Exception ex)
            {
                return Validation<string, Control>.Fail(new Seq<string> { ex.Message });
            }
        }
        
        public Eff<Option<string>> AddAssessmentControl(AssessmentControl assessmentControl)
        {
            return Eff(() =>
            {
                if (assessmentControl is null)
                return Option<string>.Some("AssessmentControl cannot be null.");
                if (assessmentControl.TestResult < 0)
                return Option<string>.Some("Test Result is out of valid range.");
                if (assessmentControl.CreatedAt == default)
                return Option<string>.Some("Created At must be set.");
                if (assessmentControl.CreatedAt < new DateTime(2024, 1, 1) || assessmentControl.CreatedAt > DateTime.UtcNow.AddYears(100))
                return Option<string>.Some("Created At is out of valid range.");
                _assessmentControls.Add(assessmentControl);
                RaiseDomainEvent(new AssessmentControlCreatedEvent(assessmentControl, "Created"));
                return Option<string>.None;
            });
        }
        public Eff<Option<string>> AddAssetControl(AssetControl assetControl)
        {
            return Eff(() =>
            {
                if (assetControl is null)
                return Option<string>.Some("AssetControl cannot be null.");
                if (assetControl.CreatedAt == default)
                return Option<string>.Some("Created At must be set.");
                if (assetControl.CreatedAt < new DateTime(2024, 1, 1) || assetControl.CreatedAt > DateTime.UtcNow.AddYears(100))
                return Option<string>.Some("Created At is out of valid range.");
                _assetControls.Add(assetControl);
                RaiseDomainEvent(new AssetControlCreatedEvent(assetControl, "Created"));
                return Option<string>.None;
            });
        }
        public Eff<Option<string>> AddCompensatingControl(CompensatingControl compensatingControl)
        {
            return Eff(() =>
            {
                if (compensatingControl is null)
                return Option<string>.Some("CompensatingControl cannot be null.");
                if (string.IsNullOrWhiteSpace(compensatingControl.Justification))
                return Option<string>.Some("Justification cannot be empty.");
                if (compensatingControl.Justification.Length > -1)
                return Option<string>.Some("Justification cannot exceed -1 characters.");
                if (string.IsNullOrWhiteSpace(compensatingControl.ImplementationDetails))
                return Option<string>.Some("Implementation Details cannot be empty.");
                if (compensatingControl.ImplementationDetails.Length > -1)
                return Option<string>.Some("Implementation Details cannot exceed -1 characters.");
                if (compensatingControl.ExpiryDate == default)
                return Option<string>.Some("Expiry Date must be set.");
                if (compensatingControl.ExpiryDate < new DateTime(2024, 1, 1) || compensatingControl.ExpiryDate > DateTime.UtcNow.AddYears(100))
                return Option<string>.Some("Expiry Date is out of valid range.");
                if (compensatingControl.Rank < 0)
                return Option<string>.Some("Rank is out of valid range.");
                if (compensatingControl.CreatedAt == default)
                return Option<string>.Some("Created At must be set.");
                if (compensatingControl.CreatedAt < new DateTime(2024, 1, 1) || compensatingControl.CreatedAt > DateTime.UtcNow.AddYears(100))
                return Option<string>.Some("Created At is out of valid range.");
                _compensatingControls.Add(compensatingControl);
                RaiseDomainEvent(new CompensatingControlCreatedEvent(compensatingControl, "Created"));
                return Option<string>.None;
            });
        }
        public Eff<Option<string>> AddControlEvidence(ControlEvidence controlEvidence)
        {
            return Eff(() =>
            {
                if (controlEvidence is null)
                return Option<string>.Some("ControlEvidence cannot be null.");
                if (controlEvidence.CreatedAt == default)
                return Option<string>.Some("Created At must be set.");
                if (controlEvidence.CreatedAt < new DateTime(2024, 1, 1) || controlEvidence.CreatedAt > DateTime.UtcNow.AddYears(100))
                return Option<string>.Some("Created At is out of valid range.");
                _controlEvidences.Add(controlEvidence);
                RaiseDomainEvent(new ControlEvidenceCreatedEvent(controlEvidence, "Created"));
                return Option<string>.None;
            });
        }
        
        public static class ControlCalculations
        {
            public static decimal CalculateTotal(int frequencydays)
            {
                return frequencydays;
            }
            
            public static int CalculateDaysBetween(DateTime effectivedate, DateTime createdat)
            {
                return (createdat - effectivedate).Days;
            }
            
        }
    }
}

