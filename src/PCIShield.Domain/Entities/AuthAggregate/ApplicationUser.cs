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
    
    public class ApplicationUser : BaseEntityEv<Guid>, IAggregateRoot, ITenantEntity
    {
        [Key]
        public Guid ApplicationUserId { get; private set; }
        
        public string? FirstName { get; private set; }
        
        public string? LastName { get; private set; }
        
        public string UserName { get; private set; }
        
        public DateTime CreatedDate { get; private set; }
        
        public Guid CreatedBy { get; private set; }
        
        public DateTime? UpdatedDate { get; private set; }
        
        public Guid? UpdatedBy { get; private set; }
        
        public bool IsLoginAllowed { get; private set; }
        
        public DateTime? LastLogin { get; private set; }
        
        public DateTime? LogoutTime { get; private set; }
        
        public DateTime? LastFailedLogin { get; private set; }
        
        public int FailedLoginCount { get; private set; }
        
        public string? Email { get; private set; }
        
        public string? Phone { get; private set; }
        
        public string? AvatarUrl { get; private set; }
        
        public bool IsUserApproved { get; private set; }
        
        public bool IsPhoneVerified { get; private set; }
        
        public bool IsEmailVerified { get; private set; }
        
        public string? ConfirmationEmail { get; private set; }
        
        public DateTime? LastPasswordChange { get; private set; }
        
        public bool IsLocked { get; private set; }
        
        public DateTime? LockedUntil { get; private set; }
        
        public bool IsDeleted { get; private set; }
        
        public bool IsUserFullyRegistered { get; private set; }
        
        public string? AvailabilityRank { get; private set; }
        
        public bool IsBanned { get; private set; }
        
        public bool IsFullyRegistered { get; private set; }
        
        public string? LastLoginIP { get; private set; }
        
        public DateTime? LastActiveAt { get; private set; }
        
        public bool? IsOnline { get; private set; }
        
        public bool? IsConnectedToSignalr { get; private set; }
        
        public DateTime? TimeLastSignalrPing { get; private set; }
        
        public bool? IsLoggedIntoApp { get; private set; }
        
        public DateTime? TimeLastLoggedToApp { get; private set; }
        
        public int? AverageResponseTime { get; private set; }
        
        public string? UserIconUrl { get; private set; }
        
        public string? UserProfileImagePath { get; private set; }
        
        public DateTime? UserBirthDate { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        
        public DateTime? UpdatedAt { get; private set; }
        
        public Guid TenantId { get; private set; }
        
        public bool? IsEmployee { get; private set; }
        
        public bool? IsErpOwner { get; private set; }
        
        public bool? IsCustomer { get; private set; }
        
        public void SetFirstName(string firstName)
        {
            FirstName = firstName;
        }
        public void SetLastName(string lastName)
        {
            LastName = lastName;
        }
        public void SetUserName(string userName)
        {
            UserName = Guard.Against.NullOrEmpty(userName, nameof(userName));
        }
        public void SetCreatedDate(DateTime createdDate)
        {
            CreatedDate = Guard.Against.OutOfSQLDateRange(createdDate, nameof(createdDate));
        }
        public void SetCreatedBy(Guid createdBy)
        {
            CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
        }
        public void SetUpdatedDate(DateTime? updatedDate)
        {
            UpdatedDate = updatedDate;
        }
        public void SetUpdatedBy(Guid? updatedBy)
        {
            UpdatedBy = updatedBy;
        }
        public void SetIsLoginAllowed(bool isLoginAllowed)
        {
            IsLoginAllowed = Guard.Against.Null(isLoginAllowed, nameof(isLoginAllowed));
        }
        public void SetLastLogin(DateTime? lastLogin)
        {
            LastLogin = lastLogin;
        }
        public void SetLogoutTime(DateTime? logoutTime)
        {
            LogoutTime = logoutTime;
        }
        public void SetLastFailedLogin(DateTime? lastFailedLogin)
        {
            LastFailedLogin = lastFailedLogin;
        }
        public void SetFailedLoginCount(int failedLoginCount)
        {
            FailedLoginCount = Guard.Against.Negative(failedLoginCount, nameof(failedLoginCount));
        }
        public void SetEmail(string email)
        {
            Email = email;
        }
        public void SetPhone(string phone)
        {
            Phone = phone;
        }
        public void SetAvatarUrl(string avatarUrl)
        {
            AvatarUrl = avatarUrl;
        }
        public void SetIsUserApproved(bool isUserApproved)
        {
            IsUserApproved = Guard.Against.Null(isUserApproved, nameof(isUserApproved));
        }
        public void SetIsPhoneVerified(bool isPhoneVerified)
        {
            IsPhoneVerified = Guard.Against.Null(isPhoneVerified, nameof(isPhoneVerified));
        }
        public void SetIsEmailVerified(bool isEmailVerified)
        {
            IsEmailVerified = Guard.Against.Null(isEmailVerified, nameof(isEmailVerified));
        }
        public void SetConfirmationEmail(string confirmationEmail)
        {
            ConfirmationEmail = confirmationEmail;
        }
        public void SetLastPasswordChange(DateTime? lastPasswordChange)
        {
            LastPasswordChange = lastPasswordChange;
        }
        public void SetIsLocked(bool isLocked)
        {
            IsLocked = Guard.Against.Null(isLocked, nameof(isLocked));
        }
        public void SetLockedUntil(DateTime? lockedUntil)
        {
            LockedUntil = lockedUntil;
        }
        public void SetIsDeleted(bool isDeleted)
        {
            IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
        }
        public void SetIsUserFullyRegistered(bool isUserFullyRegistered)
        {
            IsUserFullyRegistered = Guard.Against.Null(isUserFullyRegistered, nameof(isUserFullyRegistered));
        }
        public void SetAvailabilityRank(string availabilityRank)
        {
            AvailabilityRank = availabilityRank;
        }
        public void SetIsBanned(bool isBanned)
        {
            IsBanned = Guard.Against.Null(isBanned, nameof(isBanned));
        }
        public void SetIsFullyRegistered(bool isFullyRegistered)
        {
            IsFullyRegistered = Guard.Against.Null(isFullyRegistered, nameof(isFullyRegistered));
        }
        public void SetLastLoginIP(string lastLoginIP)
        {
            LastLoginIP = lastLoginIP;
        }
        public void SetLastActiveAt(DateTime? lastActiveAt)
        {
            LastActiveAt = lastActiveAt;
        }
        public void SetIsOnline(bool? isOnline)
        {
            IsOnline = isOnline;
        }
        public void SetIsConnectedToSignalr(bool? isConnectedToSignalr)
        {
            IsConnectedToSignalr = isConnectedToSignalr;
        }
        public void SetTimeLastSignalrPing(DateTime? timeLastSignalrPing)
        {
            TimeLastSignalrPing = timeLastSignalrPing;
        }
        public void SetIsLoggedIntoApp(bool? isLoggedIntoApp)
        {
            IsLoggedIntoApp = isLoggedIntoApp;
        }
        public void SetTimeLastLoggedToApp(DateTime? timeLastLoggedToApp)
        {
            TimeLastLoggedToApp = timeLastLoggedToApp;
        }
        public void SetAverageResponseTime(int? averageResponseTime)
        {
            AverageResponseTime = averageResponseTime;
        }
        public void SetUserIconUrl(string userIconUrl)
        {
            UserIconUrl = userIconUrl;
        }
        public void SetUserProfileImagePath(string userProfileImagePath)
        {
            UserProfileImagePath = userProfileImagePath;
        }
        public void SetUserBirthDate(DateTime? userBirthDate)
        {
            UserBirthDate = userBirthDate;
        }
        public void SetCreatedAt(DateTime createdAt)
        {
            CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
        }
        public void SetUpdatedAt(DateTime? updatedAt)
        {
            UpdatedAt = updatedAt;
        }
        public void SetTenantId(Guid tenantId)
        {
            TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
        }
        public void SetIsEmployee(bool? isEmployee)
        {
            IsEmployee = isEmployee;
        }
        public void SetIsErpOwner(bool? isErpOwner)
        {
            IsErpOwner = isErpOwner;
        }
        public void SetIsCustomer(bool? isCustomer)
        {
            IsCustomer = isCustomer;
        }
        private ApplicationUser() { }
        
        public ApplicationUser(Guid applicationUserId, string userName, DateTime createdDate, Guid createdBy, bool isLoginAllowed, int failedLoginCount, bool isUserApproved, bool isPhoneVerified, bool isEmailVerified, bool isLocked, bool isDeleted, bool isUserFullyRegistered, bool isBanned, bool isFullyRegistered, DateTime createdAt, Guid tenantId)
        {
            this.ApplicationUserId = Guard.Against.Default(applicationUserId, nameof(applicationUserId));
            this.UserName = Guard.Against.NullOrEmpty(userName, nameof(userName));
            this.CreatedDate = Guard.Against.OutOfSQLDateRange(createdDate, nameof(createdDate));
            this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
            this.IsLoginAllowed = Guard.Against.Null(isLoginAllowed, nameof(isLoginAllowed));
            this.FailedLoginCount = Guard.Against.Negative(failedLoginCount, nameof(failedLoginCount));
            this.IsUserApproved = Guard.Against.Null(isUserApproved, nameof(isUserApproved));
            this.IsPhoneVerified = Guard.Against.Null(isPhoneVerified, nameof(isPhoneVerified));
            this.IsEmailVerified = Guard.Against.Null(isEmailVerified, nameof(isEmailVerified));
            this.IsLocked = Guard.Against.Null(isLocked, nameof(isLocked));
            this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
            this.IsUserFullyRegistered = Guard.Against.Null(isUserFullyRegistered, nameof(isUserFullyRegistered));
            this.IsBanned = Guard.Against.Null(isBanned, nameof(isBanned));
            this.IsFullyRegistered = Guard.Against.Null(isFullyRegistered, nameof(isFullyRegistered));
            this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
            this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
            
        }
        public override bool Equals(object? obj) =>
        obj is ApplicationUser applicationUser && Equals(applicationUser);
        
        public bool Equals(ApplicationUser other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return ApplicationUserId.Equals(other.ApplicationUserId);
        }
        
        public override int GetHashCode() => ApplicationUserId.GetHashCode();
        
        public static bool operator !=(ApplicationUser left, ApplicationUser right) => !(left == right);
        
        public static bool operator ==(ApplicationUser left, ApplicationUser right) => left?.Equals(right) ?? right is null;
        
        private void ValidateInvariants()
        {
            if (FirstName?.Length > 255)
            throw new InvalidOperationException("FirstName cannot exceed 255 characters.");
            if (LastName?.Length > 255)
            throw new InvalidOperationException("LastName cannot exceed 255 characters.");
            if (string.IsNullOrWhiteSpace(UserName))
            throw new InvalidOperationException("UserName cannot be null or whitespace.");
            if (UserName?.Length > 255)
            throw new InvalidOperationException("UserName cannot exceed 255 characters.");
            if (CreatedDate == default)
            throw new InvalidOperationException("CreatedDate must be set.");
            if (UpdatedDate == default)
            throw new InvalidOperationException("UpdatedDate must be set.");
            if (Email?.Length > 255)
            throw new InvalidOperationException("Email cannot exceed 255 characters.");
            if (Phone?.Length > 255)
            throw new InvalidOperationException("Phone cannot exceed 255 characters.");
            if (AvatarUrl?.Length > 255)
            throw new InvalidOperationException("AvatarUrl cannot exceed 255 characters.");
            if (ConfirmationEmail?.Length > 255)
            throw new InvalidOperationException("ConfirmationEmail cannot exceed 255 characters.");
            if (AvailabilityRank?.Length > 50)
            throw new InvalidOperationException("AvailabilityRank cannot exceed 50 characters.");
            if (LastLoginIP?.Length > 50)
            throw new InvalidOperationException("LastLoginIP cannot exceed 50 characters.");
            if (UserIconUrl?.Length > 255)
            throw new InvalidOperationException("UserIconUrl cannot exceed 255 characters.");
            if (UserProfileImagePath?.Length > 255)
            throw new InvalidOperationException("UserProfileImagePath cannot exceed 255 characters.");
            if (UserBirthDate == default)
            throw new InvalidOperationException("UserBirthDate must be set.");
            if (UpdatedAt == default)
            throw new InvalidOperationException("UpdatedAt must be set.");
        }
        
    }
}

