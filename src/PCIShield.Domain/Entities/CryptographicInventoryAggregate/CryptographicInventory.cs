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
    
    public class CryptographicInventory : BaseEntityEv<Guid>, IAggregateRoot, ITrackedEntity, ITenantEntity
    {
        [Key]
        public Guid CryptographicInventoryId { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public string KeyName { get; private set; }
        
        public string KeyType { get; private set; }
        
        public string Algorithm { get; private set; }
        
        public int KeyLength { get; private set; }
        
        public string KeyLocation { get; private set; }
        
        public DateTime CreationDate { get; private set; }
        
        public DateTime? LastRotationDate { get; private set; }
        
        public DateTime NextRotationDue { get; private set; }
        
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
        public void SetKeyName(string keyName)
        {
            KeyName = Guard.Against.NullOrEmpty(keyName, nameof(keyName));
        }
        public void SetKeyType(string keyType)
        {
            KeyType = Guard.Against.NullOrEmpty(keyType, nameof(keyType));
        }
        public void SetAlgorithm(string algorithm)
        {
            Algorithm = Guard.Against.NullOrEmpty(algorithm, nameof(algorithm));
        }
        public void SetKeyLength(int keyLength)
        {
            KeyLength = Guard.Against.Negative(keyLength, nameof(keyLength));
        }
        public void SetKeyLocation(string keyLocation)
        {
            KeyLocation = Guard.Against.NullOrEmpty(keyLocation, nameof(keyLocation));
        }
        public void SetCreationDate(DateTime creationDate)
        {
            CreationDate = Guard.Against.OutOfSQLDateRange(creationDate, nameof(creationDate));
        }
        public void SetLastRotationDate(DateTime? lastRotationDate)
        {
            LastRotationDate = lastRotationDate;
        }
        public void SetNextRotationDue(DateTime nextRotationDue)
        {
            NextRotationDue = Guard.Against.OutOfSQLDateRange(nextRotationDue, nameof(nextRotationDue));
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
        
        public virtual Merchant Merchant { get; private set; }
        
        public Guid MerchantId { get; private set; }
        
        private CryptographicInventory() { }
        
        public CryptographicInventory(Guid cryptographicInventoryId, Guid merchantId, Guid tenantId, string keyName, string keyType, string algorithm, int keyLength, string keyLocation, DateTime creationDate, DateTime nextRotationDue, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
            this.CryptographicInventoryId = Guard.Against.Default(cryptographicInventoryId, nameof(cryptographicInventoryId));
            this.MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            this.KeyName = Guard.Against.NullOrEmpty(keyName, nameof(keyName));
            this.KeyType = Guard.Against.NullOrEmpty(keyType, nameof(keyType));
            this.Algorithm = Guard.Against.NullOrEmpty(algorithm, nameof(algorithm));
            this.KeyLength = Guard.Against.Negative(keyLength, nameof(keyLength));
            this.KeyLocation = Guard.Against.NullOrEmpty(keyLocation, nameof(keyLocation));
            this.CreationDate = Guard.Against.OutOfSQLDateRange(creationDate, nameof(creationDate));
            this.NextRotationDue = Guard.Against.OutOfSQLDateRange(nextRotationDue, nameof(nextRotationDue));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            
        }
        public override bool Equals(object? obj) =>
        obj is CryptographicInventory cryptographicInventory && Equals(cryptographicInventory);
        
        public bool Equals(CryptographicInventory other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return CryptographicInventoryId.Equals(other.CryptographicInventoryId);
        }
        
        public override int GetHashCode() => CryptographicInventoryId.GetHashCode();
        
        public static bool operator !=(CryptographicInventory left, CryptographicInventory right) => !(left == right);
        
        public static bool operator ==(CryptographicInventory left, CryptographicInventory right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (string.IsNullOrWhiteSpace(KeyName))
            throw new InvalidOperationException("KeyName cannot be null or whitespace.");
            if (KeyName?.Length > 200)
            throw new InvalidOperationException("KeyName cannot exceed 200 characters.");
            if (string.IsNullOrWhiteSpace(KeyType))
            throw new InvalidOperationException("KeyType cannot be null or whitespace.");
            if (KeyType?.Length > 50)
            throw new InvalidOperationException("KeyType cannot exceed 50 characters.");
            if (string.IsNullOrWhiteSpace(Algorithm))
            throw new InvalidOperationException("Algorithm cannot be null or whitespace.");
            if (Algorithm?.Length > 50)
            throw new InvalidOperationException("Algorithm cannot exceed 50 characters.");
            if (string.IsNullOrWhiteSpace(KeyLocation))
            throw new InvalidOperationException("KeyLocation cannot be null or whitespace.");
            if (KeyLocation?.Length > 200)
            throw new InvalidOperationException("KeyLocation cannot exceed 200 characters.");
            if (CreationDate == default)
            throw new InvalidOperationException("CreationDate must be set.");
            if (LastRotationDate == default)
            throw new InvalidOperationException("LastRotationDate must be set.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
    }
}

