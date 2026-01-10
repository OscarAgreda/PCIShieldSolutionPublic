using System;
using System.Collections.Generic;
using System.Linq;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using Ardalis.GuardClauses;

namespace PCIShield.Domain.ModelEntityDto
{
    
    public class ApplicationUserEntityDto : IEntityDto
    {
        public Guid ApplicationUserId { get;  set; }
        
        public string? FirstName { get;  set; }
        
        public string? LastName { get;  set; }
        
        public string UserName { get;  set; }
        
        public DateTime CreatedDate { get;  set; }
        
        public Guid CreatedBy { get;  set; }
        
        public DateTime? UpdatedDate { get;  set; }
        
        public Guid? UpdatedBy { get;  set; }
        
        public bool IsLoginAllowed { get;  set; }
        
        public DateTime? LastLogin { get;  set; }
        
        public DateTime? LogoutTime { get;  set; }
        
        public DateTime? LastFailedLogin { get;  set; }
        
        public int FailedLoginCount { get;  set; }
        
        public string? Email { get;  set; }
        
        public string? Phone { get;  set; }
        
        public string? AvatarUrl { get;  set; }
        
        public bool IsUserApproved { get;  set; }
        
        public bool IsPhoneVerified { get;  set; }
        
        public bool IsEmailVerified { get;  set; }
        
        public string? ConfirmationEmail { get;  set; }
        
        public DateTime? LastPasswordChange { get;  set; }
        
        public bool IsLocked { get;  set; }
        
        public DateTime? LockedUntil { get;  set; }
        
        public bool IsDeleted { get;  set; }
        
        public bool IsUserFullyRegistered { get;  set; }
        
        public string? AvailabilityRank { get;  set; }
        
        public bool IsBanned { get;  set; }
        
        public bool IsFullyRegistered { get;  set; }
        
        public string? LastLoginIP { get;  set; }
        
        public DateTime? LastActiveAt { get;  set; }
        
        public bool? IsOnline { get;  set; }
        
        public bool? IsConnectedToSignalr { get;  set; }
        
        public DateTime? TimeLastSignalrPing { get;  set; }
        
        public bool? IsLoggedIntoApp { get;  set; }
        
        public DateTime? TimeLastLoggedToApp { get;  set; }
        
        public int? AverageResponseTime { get;  set; }
        
        public string? UserIconUrl { get;  set; }
        
        public string? UserProfileImagePath { get;  set; }
        
        public DateTime? UserBirthDate { get;  set; }
        
        public DateTime CreatedAt { get;  set; }
        
        public DateTime? UpdatedAt { get;  set; }
        
        public Guid TenantId { get;  set; }
        
        public bool? IsEmployee { get;  set; }
        
        public bool? IsErpOwner { get;  set; }
        
        public bool? IsCustomer { get;  set; }
        
        public ApplicationUserEntityDto() {}
        
    }
}

