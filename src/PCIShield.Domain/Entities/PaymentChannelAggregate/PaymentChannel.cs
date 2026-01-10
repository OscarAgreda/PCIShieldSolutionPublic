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
    
    public class PaymentChannel : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public Guid PaymentChannelId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public string ChannelCode { get; private set; }
        
        public string ChannelName { get; private set; }
        
        public int ChannelType { get; private set; }
        
        public decimal ProcessingVolume { get; private set; }
        
        public bool IsInScope { get; private set; }
        
        public bool TokenizationEnabled { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        
        public Guid CreatedBy { get; private set; }
        
        public DateTime? UpdatedAt { get; private set; }
        
        public Guid? UpdatedBy { get; private set; }
        
        public bool IsDeleted { get; private set; }
        
        public void SetTenantId(Guid tenantId)
        {
            TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
        }
        public void SetMerchantId(Guid merchantId)
        {
            MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
        }
        public void SetChannelCode(string channelCode)
        {
            ChannelCode = Guard.Against.NullOrEmpty(channelCode, nameof(channelCode));
        }
        public void SetChannelName(string channelName)
        {
            ChannelName = Guard.Against.NullOrEmpty(channelName, nameof(channelName));
        }
        public void SetChannelType(int channelType)
        {
            ChannelType = Guard.Against.Negative(channelType, nameof(channelType));
        }
        public void SetProcessingVolume(decimal processingVolume)
        {
            ProcessingVolume = Guard.Against.Negative(processingVolume, nameof(processingVolume));
        }
        public void SetIsInScope(bool isInScope)
        {
            IsInScope = Guard.Against.Null(isInScope, nameof(isInScope));
        }
        public void SetTokenizationEnabled(bool tokenizationEnabled)
        {
            TokenizationEnabled = Guard.Against.Null(tokenizationEnabled, nameof(tokenizationEnabled));
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
        
        public void UpdateMerchantForPaymentChannel(Guid newMerchantId)
        {
            Guard.Against.Default(newMerchantId, nameof(newMerchantId));
            if (newMerchantId == MerchantId)
            {
                return;
            }
            
            MerchantId = newMerchantId;
            var paymentChannelUpdatedEvent = new PaymentChannelUpdatedEvent(this, "UpdatedEvent merchant");
            Events.Add(paymentChannelUpdatedEvent);
        }
        
        public virtual Merchant Merchant { get; private set; }
        
        public Guid MerchantId { get; private set; }
        
        private PaymentChannel() { }
        
        public PaymentChannel(Guid paymentChannelId, Guid merchantId, Guid tenantId, string channelCode, string channelName, int channelType, decimal processingVolume, bool isInScope, bool tokenizationEnabled, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            this.PaymentChannelId = Guard.Against.Default(paymentChannelId, nameof(paymentChannelId));
            this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.ChannelCode = Guard.Against.NullOrEmpty(channelCode, nameof(channelCode));
            this.ChannelName = Guard.Against.NullOrEmpty(channelName, nameof(channelName));
            this.ChannelType = Guard.Against.Negative(channelType, nameof(channelType));
            this.ProcessingVolume = Guard.Against.Negative(processingVolume, nameof(processingVolume));
            this.IsInScope = Guard.Against.Null(isInScope, nameof(isInScope));
            this.TokenizationEnabled = Guard.Against.Null(tokenizationEnabled, nameof(tokenizationEnabled));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            
            _paymentPages = new();
            
        }
        private readonly List<PaymentPage> _paymentPages = new();
        public IEnumerable<PaymentPage> PaymentPages => _paymentPages.AsReadOnly();
        
        public override bool Equals(object? obj) =>
        obj is PaymentChannel paymentChannel && Equals(paymentChannel);
        
        public bool Equals(PaymentChannel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return PaymentChannelId.Equals(other.PaymentChannelId);
        }
        
        public override int GetHashCode() => PaymentChannelId.GetHashCode();
        
        public static bool operator !=(PaymentChannel left, PaymentChannel right) => !(left == right);
        
        public static bool operator ==(PaymentChannel left, PaymentChannel right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(ChannelCode))
            throw new InvalidOperationException("ChannelCode cannot be null or whitespace.");
            if (ChannelCode?.Length > 30)
            throw new InvalidOperationException("ChannelCode cannot exceed 30 characters.");
            if (string.IsNullOrWhiteSpace(ChannelName))
            throw new InvalidOperationException("ChannelName cannot be null or whitespace.");
            if (ChannelName?.Length > 100)
            throw new InvalidOperationException("ChannelName cannot exceed 100 characters.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
        private void RaiseDomainEvent(BaseDomainEvent domainEvent)
        {
            Events.Add(domainEvent);
        }
        public static Eff<Validation<string, PaymentChannel>> Create(
        Guid paymentChannelId,
        Guid merchantId,
        Guid tenantId,
        string channelCode,
        string channelName,
        int channelType,
        decimal processingVolume,
        bool isInScope,
        bool tokenizationEnabled,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            return Eff(() => CreatePaymentChannel(
            paymentChannelId,
            merchantId,
            tenantId,
            channelCode,
            channelName,
            channelType,
            processingVolume,
            isInScope,
            tokenizationEnabled,
            createdAt,
            createdBy,
            isDeleted
            ));
        }
        
        private static Validation<string, PaymentChannel> CreatePaymentChannel(
        Guid paymentChannelId,
        Guid merchantId,
        Guid tenantId,
        string channelCode,
        string channelName,
        int channelType,
        decimal processingVolume,
        bool isInScope,
        bool tokenizationEnabled,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            try
            {
                var now = DateTime.UtcNow;
                isDeleted = false;
                
                var paymentChannel = new PaymentChannel
                ( paymentChannelId,
                merchantId,
                tenantId,
                channelCode,
                channelName,
                channelType,
                processingVolume,
                isInScope,
                tokenizationEnabled,
                createdAt,
                createdBy,
                isDeleted)
                ;
                
                return Validation<string, PaymentChannel>.Success(paymentChannel);
            }
            catch (Exception ex)
            {
                return Validation<string, PaymentChannel>.Fail(new Seq<string> { ex.Message });
            }
        }
        public static Eff<Validation<string, PaymentChannel>> Update(
        PaymentChannel existingPaymentChannel,
        Guid paymentChannelId,
        Guid tenantId,
        Guid merchantId,
        string channelCode,
        string channelName,
        int channelType,
        decimal processingVolume,
        bool isInScope,
        bool tokenizationEnabled,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            return Eff(() => UpdatePaymentChannel(
            existingPaymentChannel,
            paymentChannelId,
            tenantId,
            merchantId,
            channelCode,
            channelName,
            channelType,
            processingVolume,
            isInScope,
            tokenizationEnabled,
            createdAt,
            createdBy,
            updatedAt.ToOption().ToNullable(),
            updatedBy.ToOption().ToNullable(),
            isDeleted
            ));
        }
        
        private static Validation<string, PaymentChannel> UpdatePaymentChannel(
        PaymentChannel paymentChannel,
        Guid paymentChannelId,
        Guid tenantId,
        Guid merchantId,
        string channelCode,
        string channelName,
        int channelType,
        decimal processingVolume,
        bool isInScope,
        bool tokenizationEnabled,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            try
            {
                
                paymentChannel.PaymentChannelId = paymentChannelId;
                paymentChannel.TenantId = tenantId;
                paymentChannel.MerchantId = merchantId;
                paymentChannel.ChannelCode = channelCode;
                paymentChannel.ChannelName = channelName;
                paymentChannel.ChannelType = channelType;
                paymentChannel.ProcessingVolume = processingVolume;
                paymentChannel.IsInScope = isInScope;
                paymentChannel.TokenizationEnabled = tokenizationEnabled;
                paymentChannel.CreatedAt = createdAt;
                paymentChannel.CreatedBy = createdBy;
                paymentChannel.UpdatedAt = updatedAt;
                paymentChannel.UpdatedBy = updatedBy;
                paymentChannel.IsDeleted = isDeleted;
                paymentChannel.RaiseDomainEvent(new PaymentChannelUpdatedEvent(paymentChannel, "Updated"));
                
                return Success<string, PaymentChannel>(paymentChannel);
            }
            catch (Exception ex)
            {
                return Validation<string, PaymentChannel>.Fail(new Seq<string> { ex.Message });
            }
        }
        
        public Eff<Option<string>> AddPaymentPage(PaymentPage paymentPage)
        {
            return Eff(() =>
            {
                if (paymentPage is null)
                return Option<string>.Some("PaymentPage cannot be null.");
                if (string.IsNullOrWhiteSpace(paymentPage.PageUrl))
                return Option<string>.Some("Page Url cannot be empty.");
                if (paymentPage.PageUrl.Length > 500)
                return Option<string>.Some("Page Url cannot exceed 500 characters.");
                if (string.IsNullOrWhiteSpace(paymentPage.PageName))
                return Option<string>.Some("Page Name cannot be empty.");
                if (paymentPage.PageName.Length > 200)
                return Option<string>.Some("Page Name cannot exceed 200 characters.");
                if (paymentPage.CreatedAt == default)
                return Option<string>.Some("Created At must be set.");
                if (paymentPage.CreatedAt < new DateTime(2024, 1, 1) || paymentPage.CreatedAt > DateTime.UtcNow.AddYears(100))
                return Option<string>.Some("Created At is out of valid range.");
                _paymentPages.Add(paymentPage);
                RaiseDomainEvent(new PaymentPageCreatedEvent(paymentPage, "Created"));
                return Option<string>.None;
            });
        }
        
        public static class PaymentChannelCalculations
        {
            public static decimal CalculateTotal(int channeltype, decimal processingvolume)
            {
                return channeltype + processingvolume;
            }
            
            public static decimal CalculateAverage(int channeltype, decimal processingvolume)
            {
                return (channeltype + processingvolume) / 2;
            }
            
            public static decimal CalculateChannelTypePerProcessingVolume(int channeltype, decimal processingvolume)
            {
                if (processingvolume == 0)
                throw new DivideByZeroException("Cannot calculate rate: ProcessingVolume is zero.");
                return (decimal)channeltype / processingvolume;
            }
            
            public static int CalculateDaysBetween(DateTime createdat, DateTime updatedat)
            {
                return (updatedat - createdat).Days;
            }
            
        }
    }
}

