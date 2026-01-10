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
    
    public class PaymentPage : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public Guid PaymentPageId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public string PageUrl { get; private set; }
        
        public string PageName { get; private set; }
        
        public bool IsActive { get; private set; }
        
        public DateTime? LastScriptInventory { get; private set; }
        
        public string? ScriptIntegrityHash { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        
        public Guid CreatedBy { get; private set; }
        
        public DateTime? UpdatedAt { get; private set; }
        
        public Guid? UpdatedBy { get; private set; }
        
        public bool IsDeleted { get; private set; }
        
        public void SetTenantId(Guid tenantId)
        {
            TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
        }
        public void SetPaymentChannelId(Guid paymentChannelId)
        {
            PaymentChannelId = Guard.Against.Default(paymentChannelId, nameof(paymentChannelId));
        }
        public void SetPageUrl(string pageUrl)
        {
            PageUrl = Guard.Against.NullOrEmpty(pageUrl, nameof(pageUrl));
        }
        public void SetPageName(string pageName)
        {
            PageName = Guard.Against.NullOrEmpty(pageName, nameof(pageName));
        }
        public void SetIsActive(bool isActive)
        {
            IsActive = Guard.Against.Null(isActive, nameof(isActive));
        }
        public void SetLastScriptInventory(DateTime? lastScriptInventory)
        {
            LastScriptInventory = lastScriptInventory;
        }
        public void SetScriptIntegrityHash(string scriptIntegrityHash)
        {
            ScriptIntegrityHash = scriptIntegrityHash;
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
        
        public virtual PaymentChannel PaymentChannel { get; private set; }
        
        public Guid PaymentChannelId { get; private set; }
        
        private PaymentPage() { }
        
        public PaymentPage(Guid paymentPageId, Guid paymentChannelId, Guid tenantId, string pageUrl, string pageName, bool isActive, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            this.PaymentPageId = Guard.Against.Default(paymentPageId, nameof(paymentPageId));
            this.PaymentChannelId = Guard.Against.Default(paymentChannelId, nameof(paymentChannelId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.PageUrl = Guard.Against.NullOrEmpty(pageUrl, nameof(pageUrl));
            this.PageName = Guard.Against.NullOrEmpty(pageName, nameof(pageName));
            this.IsActive = Guard.Against.Null(isActive, nameof(isActive));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            
            _scripts = new();
            
        }
        private readonly List<Script> _scripts = new();
        public IEnumerable<Script> Scripts => _scripts.AsReadOnly();
        
        public override bool Equals(object? obj) =>
        obj is PaymentPage paymentPage && Equals(paymentPage);
        
        public bool Equals(PaymentPage other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return PaymentPageId.Equals(other.PaymentPageId);
        }
        
        public override int GetHashCode() => PaymentPageId.GetHashCode();
        
        public static bool operator !=(PaymentPage left, PaymentPage right) => !(left == right);
        
        public static bool operator ==(PaymentPage left, PaymentPage right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(PageUrl))
            throw new InvalidOperationException("PageUrl cannot be null or whitespace.");
            if (PageUrl?.Length > 500)
            throw new InvalidOperationException("PageUrl cannot exceed 500 characters.");
            if (string.IsNullOrWhiteSpace(PageName))
            throw new InvalidOperationException("PageName cannot be null or whitespace.");
            if (PageName?.Length > 200)
            throw new InvalidOperationException("PageName cannot exceed 200 characters.");
            if (ScriptIntegrityHash?.Length > 128)
            throw new InvalidOperationException("ScriptIntegrityHash cannot exceed 128 characters.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
    }
}

