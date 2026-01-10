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
    
    public class ScanSchedule : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public Guid ScanScheduleId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public int ScanType { get; private set; }
        
        public string Frequency { get; private set; }
        
        public DateTime NextScanDate { get; private set; }
        
        public TimeSpan? BlackoutStart { get; private set; }
        
        public TimeSpan? BlackoutEnd { get; private set; }
        
        public bool IsEnabled { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        
        public Guid CreatedBy { get; private set; }
        
        public DateTime? UpdatedAt { get; private set; }
        
        public Guid? UpdatedBy { get; private set; }
        
        public bool IsDeleted { get; private set; }
        
        public void SetTenantId(Guid tenantId)
        {
            TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
        }
        public void SetAssetId(Guid assetId)
        {
            AssetId = Guard.Against.Default(assetId, nameof(assetId));
        }
        public void SetScanType(int scanType)
        {
            ScanType = Guard.Against.Negative(scanType, nameof(scanType));
        }
        public void SetFrequency(string frequency)
        {
            Frequency = Guard.Against.NullOrEmpty(frequency, nameof(frequency));
        }
        public void SetNextScanDate(DateTime nextScanDate)
        {
            NextScanDate = Guard.Against.OutOfSQLDateRange(nextScanDate, nameof(nextScanDate));
        }
        public void SetBlackoutStart(TimeSpan blackoutStart)
        {
            BlackoutStart = blackoutStart;
        }
        public void SetBlackoutEnd(TimeSpan blackoutEnd)
        {
            BlackoutEnd = blackoutEnd;
        }
        public void SetIsEnabled(bool isEnabled)
        {
            IsEnabled = Guard.Against.Null(isEnabled, nameof(isEnabled));
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
        
        public void UpdateAssetForScanSchedule(Guid newAssetId)
        {
            Guard.Against.Default(newAssetId, nameof(newAssetId));
            if (newAssetId == AssetId)
            {
                return;
            }
            
            AssetId = newAssetId;
            var scanScheduleUpdatedEvent = new ScanScheduleUpdatedEvent(this, "UpdatedEvent asset");
            Events.Add(scanScheduleUpdatedEvent);
        }
        
        public virtual Asset Asset { get; private set; }
        
        public Guid AssetId { get; private set; }
        
        private ScanSchedule() { }
        
        public ScanSchedule(Guid scanScheduleId, Guid assetId, Guid tenantId, int scanType, string frequency, DateTime nextScanDate, bool isEnabled, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            this.ScanScheduleId = Guard.Against.Default(scanScheduleId, nameof(scanScheduleId));
            this.AssetId = Guard.Against.Default(assetId, nameof(assetId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.ScanType = Guard.Against.Negative(scanType, nameof(scanType));
            this.Frequency = Guard.Against.NullOrEmpty(frequency, nameof(frequency));
            this.NextScanDate = Guard.Against.OutOfSQLDateRange(nextScanDate, nameof(nextScanDate));
            this.IsEnabled = Guard.Against.Null(isEnabled, nameof(isEnabled));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            
        }
        public override bool Equals(object? obj) =>
        obj is ScanSchedule scanSchedule && Equals(scanSchedule);
        
        public bool Equals(ScanSchedule other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return ScanScheduleId.Equals(other.ScanScheduleId);
        }
        
        public override int GetHashCode() => ScanScheduleId.GetHashCode();
        
        public static bool operator !=(ScanSchedule left, ScanSchedule right) => !(left == right);
        
        public static bool operator ==(ScanSchedule left, ScanSchedule right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(Frequency))
            throw new InvalidOperationException("Frequency cannot be null or whitespace.");
            if (Frequency?.Length > 50)
            throw new InvalidOperationException("Frequency cannot exceed 50 characters.");
            if (NextScanDate == default)
            throw new InvalidOperationException("NextScanDate must be set.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
        private void RaiseDomainEvent(BaseDomainEvent domainEvent)
        {
            Events.Add(domainEvent);
        }
        public static Eff<Validation<string, ScanSchedule>> Create(
        Guid scanScheduleId,
        Guid assetId,
        Guid tenantId,
        int scanType,
        string frequency,
        DateTime nextScanDate,
        bool isEnabled,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            return Eff(() => CreateScanSchedule(
            scanScheduleId,
            assetId,
            tenantId,
            scanType,
            frequency,
            nextScanDate,
            isEnabled,
            createdAt,
            createdBy,
            isDeleted
            ));
        }
        
        private static Validation<string, ScanSchedule> CreateScanSchedule(
        Guid scanScheduleId,
        Guid assetId,
        Guid tenantId,
        int scanType,
        string frequency,
        DateTime nextScanDate,
        bool isEnabled,
        DateTime createdAt,
        Guid createdBy,
        bool isDeleted
        )
        {
            try
            {
                var now = DateTime.UtcNow;
                isDeleted = false;
                
                var scanSchedule = new ScanSchedule
                ( scanScheduleId,
                assetId,
                tenantId,
                scanType,
                frequency,
                nextScanDate,
                isEnabled,
                createdAt,
                createdBy,
                isDeleted)
                ;
                
                return Validation<string, ScanSchedule>.Success(scanSchedule);
            }
            catch (Exception ex)
            {
                return Validation<string, ScanSchedule>.Fail(new Seq<string> { ex.Message });
            }
        }
        public static Eff<Validation<string, ScanSchedule>> Update(
        ScanSchedule existingScanSchedule,
        Guid scanScheduleId,
        Guid tenantId,
        Guid assetId,
        int scanType,
        string frequency,
        DateTime nextScanDate,
        TimeSpan blackoutStart,
        TimeSpan blackoutEnd,
        bool isEnabled,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            return Eff(() => UpdateScanSchedule(
            existingScanSchedule,
            scanScheduleId,
            tenantId,
            assetId,
            scanType,
            frequency,
            nextScanDate,
            blackoutStart,
            blackoutEnd,
            isEnabled,
            createdAt,
            createdBy,
            updatedAt.ToOption().ToNullable(),
            updatedBy.ToOption().ToNullable(),
            isDeleted
            ));
        }
        
        private static Validation<string, ScanSchedule> UpdateScanSchedule(
        ScanSchedule scanSchedule,
        Guid scanScheduleId,
        Guid tenantId,
        Guid assetId,
        int scanType,
        string frequency,
        DateTime nextScanDate,
        TimeSpan blackoutStart,
        TimeSpan blackoutEnd,
        bool isEnabled,
        DateTime createdAt,
        Guid createdBy,
        DateTime? updatedAt,
        Guid? updatedBy,
        bool isDeleted
        )
        {
            try
            {
                
                scanSchedule.ScanScheduleId = scanScheduleId;
                scanSchedule.TenantId = tenantId;
                scanSchedule.AssetId = assetId;
                scanSchedule.ScanType = scanType;
                scanSchedule.Frequency = frequency;
                scanSchedule.NextScanDate = nextScanDate;
                scanSchedule.BlackoutStart = blackoutStart;
                scanSchedule.BlackoutEnd = blackoutEnd;
                scanSchedule.IsEnabled = isEnabled;
                scanSchedule.CreatedAt = createdAt;
                scanSchedule.CreatedBy = createdBy;
                scanSchedule.UpdatedAt = updatedAt;
                scanSchedule.UpdatedBy = updatedBy;
                scanSchedule.IsDeleted = isDeleted;
                scanSchedule.RaiseDomainEvent(new ScanScheduleUpdatedEvent(scanSchedule, "Updated"));
                
                return Success<string, ScanSchedule>(scanSchedule);
            }
            catch (Exception ex)
            {
                return Validation<string, ScanSchedule>.Fail(new Seq<string> { ex.Message });
            }
        }
        public static class ScanScheduleCalculations
        {
            public static decimal CalculateTotal(int scantype)
            {
                return scantype;
            }
            
            public static int CalculateDaysBetween(DateTime nextscandate, DateTime createdat)
            {
                return (createdat - nextscandate).Days;
            }
            
        }
    }
}

