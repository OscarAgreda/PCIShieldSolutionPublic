using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Ardalis.GuardClauses;
using System.Text.Json.Serialization;
using PCIShield.BlazorMauiShared;
using PCIShieldLib.SharedKernel;
using PCIShieldLib.SharedKernel.Interfaces;
using LanguageExt;
using LanguageExt.Common;
using ReactiveUI;
using static LanguageExt.Prelude;
using System.Reactive.Subjects;
using System.Text.Json;

using Unit = LanguageExt.Unit;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Concurrent;
namespace PCIShield.Domain.ModelsDto
{
    public static class ApplicationUserFactory
    {
        public static ApplicationUserDto CreateNewFromTemplate(ApplicationUserDto template)
        {
            var now = DateTime.UtcNow;

            var newApplicationUser = new ApplicationUserDto
            {
                ApplicationUserId = Guid.Empty,
                CreatedDate = now,
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                IsLoginAllowed = false,
                IsUserApproved = false,
                IsPhoneVerified = false,
                IsEmailVerified = false,
                IsLocked = false,
                IsDeleted = false,
                IsUserFullyRegistered = false,
                IsBanned = false,
                IsFullyRegistered = false,
                IsOnline = false,
                IsConnectedToSignalr = false,
                IsLoggedIntoApp = false,
                IsEmployee = false,
                IsErpOwner = false,
                IsCustomer = false,
            };

            return newApplicationUser;
        }

        public static ApplicationUserDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new ApplicationUserDto
            {
                ApplicationUserId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                TenantId = Guid.Empty,
                CreatedDate = now,
                IsDeleted = false,
            };
        }
    }
    public static class ApplicationUserExtensions
    {
        public static ApplicationUserDto CloneAsNew(this ApplicationUserDto template)
        {
            return ApplicationUserFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class ApplicationUserDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<ApplicationUserDto>,ISnapshotable<ApplicationUserDto>,IChangeObservable
    {
        public Guid  ApplicationUserId { get;  set; }

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
        public ApplicationUserDto Clone()
        {
            return new ApplicationUserDto
            {
                ApplicationUserId = this.ApplicationUserId,
                FirstName = this.FirstName,
                LastName = this.LastName,
                UserName = this.UserName,
                CreatedDate = this.CreatedDate,
                CreatedBy = this.CreatedBy,
                UpdatedDate = this.UpdatedDate,
                UpdatedBy = this.UpdatedBy,
                IsLoginAllowed = this.IsLoginAllowed,
                LastLogin = this.LastLogin,
                LogoutTime = this.LogoutTime,
                LastFailedLogin = this.LastFailedLogin,
                FailedLoginCount = this.FailedLoginCount,
                Email = this.Email,
                Phone = this.Phone,
                AvatarUrl = this.AvatarUrl,
                IsUserApproved = this.IsUserApproved,
                IsPhoneVerified = this.IsPhoneVerified,
                IsEmailVerified = this.IsEmailVerified,
                ConfirmationEmail = this.ConfirmationEmail,
                LastPasswordChange = this.LastPasswordChange,
                IsLocked = this.IsLocked,
                LockedUntil = this.LockedUntil,
                IsDeleted = this.IsDeleted,
                IsUserFullyRegistered = this.IsUserFullyRegistered,
                AvailabilityRank = this.AvailabilityRank,
                IsBanned = this.IsBanned,
                IsFullyRegistered = this.IsFullyRegistered,
                LastLoginIP = this.LastLoginIP,
                LastActiveAt = this.LastActiveAt,
                IsOnline = this.IsOnline,
                IsConnectedToSignalr = this.IsConnectedToSignalr,
                TimeLastSignalrPing = this.TimeLastSignalrPing,
                IsLoggedIntoApp = this.IsLoggedIntoApp,
                TimeLastLoggedToApp = this.TimeLastLoggedToApp,
                AverageResponseTime = this.AverageResponseTime,
                UserIconUrl = this.UserIconUrl,
                UserProfileImagePath = this.UserProfileImagePath,
                UserBirthDate = this.UserBirthDate,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt,
                TenantId = this.TenantId,
                IsEmployee = this.IsEmployee,
                IsErpOwner = this.IsErpOwner,
                IsCustomer = this.IsCustomer,
            };
        }
        public ApplicationUserDto() { }
        public ApplicationUserDto(Guid applicationUserId, string userName, DateTime createdDate, Guid createdBy, bool isLoginAllowed, int failedLoginCount, bool isUserApproved, bool isPhoneVerified, bool isEmailVerified, bool isLocked, bool isDeleted, bool isUserFullyRegistered, bool isBanned, bool isFullyRegistered, DateTime createdAt, Guid tenantId)
        {
                AuditEntityId = applicationUserId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
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
        [JsonIgnore]
        private Option<ApplicationUserDto> _snapshot = None;

        public Option<ApplicationUserDto> CurrentSnapshot => _snapshot;

        public void TakeSnapshot()
        {
            _snapshot = Some(Clone());
        }

        [NotMapped]
        [JsonIgnore]
        public bool IsDirty =>
            _snapshot.Match(
                Some: snap =>
                    !Compare(snap).Match(
                        Right: equal => equal,
                        Left: _ => false
                    ),
                None: () => true
            );

        public Either<Error, Unit> RestoreSnapshot() =>
            _snapshot.Match(
                Some: s =>
                {
                    CopyFrom(s);
                    AuditDataBeforeChanged = JsonSerializer.Serialize(this);
                    AuditDataAfterChanged = JsonSerializer.Serialize(s);
                    AuditTimeStamp = DateTime.UtcNow;
                    AuditAction = "Restored";
                    NotifyChange();
                    return Right<Error, Unit>(LanguageExt.Unit.Default);
                },
                None: () => Left<Error, Unit>(Error.New("No snapshot exists to restore"))
            );

        private void CopyFrom(ApplicationUserDto source)
        {
        ApplicationUserId = source.ApplicationUserId;
        FirstName = source.FirstName;
        LastName = source.LastName;
        UserName = source.UserName;
        CreatedDate = source.CreatedDate;
        CreatedBy = source.CreatedBy;
        UpdatedDate = source.UpdatedDate;
        UpdatedBy = source.UpdatedBy;
        IsLoginAllowed = source.IsLoginAllowed;
        LastLogin = source.LastLogin;
        LogoutTime = source.LogoutTime;
        LastFailedLogin = source.LastFailedLogin;
        FailedLoginCount = source.FailedLoginCount;
        Email = source.Email;
        Phone = source.Phone;
        AvatarUrl = source.AvatarUrl;
        IsUserApproved = source.IsUserApproved;
        IsPhoneVerified = source.IsPhoneVerified;
        IsEmailVerified = source.IsEmailVerified;
        ConfirmationEmail = source.ConfirmationEmail;
        LastPasswordChange = source.LastPasswordChange;
        IsLocked = source.IsLocked;
        LockedUntil = source.LockedUntil;
        IsDeleted = source.IsDeleted;
        IsUserFullyRegistered = source.IsUserFullyRegistered;
        AvailabilityRank = source.AvailabilityRank;
        IsBanned = source.IsBanned;
        IsFullyRegistered = source.IsFullyRegistered;
        LastLoginIP = source.LastLoginIP;
        LastActiveAt = source.LastActiveAt;
        IsOnline = source.IsOnline;
        IsConnectedToSignalr = source.IsConnectedToSignalr;
        TimeLastSignalrPing = source.TimeLastSignalrPing;
        IsLoggedIntoApp = source.IsLoggedIntoApp;
        TimeLastLoggedToApp = source.TimeLastLoggedToApp;
        AverageResponseTime = source.AverageResponseTime;
        UserIconUrl = source.UserIconUrl;
        UserProfileImagePath = source.UserProfileImagePath;
        UserBirthDate = source.UserBirthDate;
        CreatedAt = source.CreatedAt;
        UpdatedAt = source.UpdatedAt;
        TenantId = source.TenantId;
        IsEmployee = source.IsEmployee;
        IsErpOwner = source.IsErpOwner;
        IsCustomer = source.IsCustomer;
        }
        [JsonIgnore]
        private readonly BehaviorSubject<DtoState> _stateSubject
            = new(new DtoState(false, false, null));

        [JsonIgnore]
        private readonly Subject<Unit> _changes = new();

        [JsonIgnore]
        public IObservable<Unit> Changes => _changes.AsObservable();

        [JsonIgnore]
        public IObservable<DtoState> StateChanges => _stateSubject.AsObservable();

        public void NotifyChange()
        {
            _changes.OnNext(LanguageExt.Unit.Default);
            UpdateState();
        }

        private void UpdateState()
        {
            GetState().Match(
                Right: newState => _stateSubject.OnNext(newState),
                Left: err => _stateSubject.OnError(new Exception(err.Message))
            );
        }
        public Either<Error, bool> Compare(ApplicationUserDto other)
        {
            return CompareBasicFields(other).Bind(basicSame =>
            {
                if (!basicSame)
                {
                    return Right<Error, bool>(false);
                }
                else
                {
                    return Right<Error, bool>(true);
                }
            });
        }

        private Either<Error, bool> CompareBasicFields(ApplicationUserDto other)
        {
            bool same =
                (ApplicationUserId == other.ApplicationUserId) &&
                StringComparer.OrdinalIgnoreCase.Equals(FirstName, other.FirstName) &&
                StringComparer.OrdinalIgnoreCase.Equals(LastName, other.LastName) &&
                StringComparer.OrdinalIgnoreCase.Equals(UserName, other.UserName) &&
                (CreatedDate == other.CreatedDate) &&
                (CreatedBy == other.CreatedBy) &&
                (UpdatedDate == other.UpdatedDate) &&
                (UpdatedBy == other.UpdatedBy) &&
                (IsLoginAllowed == other.IsLoginAllowed) &&
                (LastLogin == other.LastLogin) &&
                (LogoutTime == other.LogoutTime) &&
                (LastFailedLogin == other.LastFailedLogin) &&
                (FailedLoginCount == other.FailedLoginCount) &&
                StringComparer.OrdinalIgnoreCase.Equals(Email, other.Email) &&
                StringComparer.OrdinalIgnoreCase.Equals(Phone, other.Phone) &&
                StringComparer.OrdinalIgnoreCase.Equals(AvatarUrl, other.AvatarUrl) &&
                (IsUserApproved == other.IsUserApproved) &&
                (IsPhoneVerified == other.IsPhoneVerified) &&
                (IsEmailVerified == other.IsEmailVerified) &&
                StringComparer.OrdinalIgnoreCase.Equals(ConfirmationEmail, other.ConfirmationEmail) &&
                (LastPasswordChange == other.LastPasswordChange) &&
                (IsLocked == other.IsLocked) &&
                (LockedUntil == other.LockedUntil) &&
                (IsDeleted == other.IsDeleted) &&
                (IsUserFullyRegistered == other.IsUserFullyRegistered) &&
                StringComparer.OrdinalIgnoreCase.Equals(AvailabilityRank, other.AvailabilityRank) &&
                (IsBanned == other.IsBanned) &&
                (IsFullyRegistered == other.IsFullyRegistered) &&
                StringComparer.OrdinalIgnoreCase.Equals(LastLoginIP, other.LastLoginIP) &&
                (LastActiveAt == other.LastActiveAt) &&
                (IsOnline == other.IsOnline) &&
                (IsConnectedToSignalr == other.IsConnectedToSignalr) &&
                (TimeLastSignalrPing == other.TimeLastSignalrPing) &&
                (IsLoggedIntoApp == other.IsLoggedIntoApp) &&
                (TimeLastLoggedToApp == other.TimeLastLoggedToApp) &&
                (AverageResponseTime == other.AverageResponseTime) &&
                StringComparer.OrdinalIgnoreCase.Equals(UserIconUrl, other.UserIconUrl) &&
                StringComparer.OrdinalIgnoreCase.Equals(UserProfileImagePath, other.UserProfileImagePath) &&
                (UserBirthDate == other.UserBirthDate) &&
                (CreatedAt == other.CreatedAt) &&
                (UpdatedAt == other.UpdatedAt) &&
                (TenantId == other.TenantId) &&
                (IsEmployee == other.IsEmployee) &&
                (IsErpOwner == other.IsErpOwner) &&
                (IsCustomer == other.IsCustomer) ;
            return Right<Error, bool>(same);
        }
        private bool CompareList<T>(List<T> current, List<T> other)
        {
            if (ReferenceEquals(current, other))
            {
                return true;
            }
            if (current.Count != other.Count)
            {
                return false;
            }
            for (int i = 0; i < current.Count; i++)
            {
                if (!Equals(current[i], other[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public Either<Error, DtoState> GetState()
        {
            var state = new DtoState(
                IsDirty: IsDirty,
                HasChanges: _changes.HasObservers,
                LastModified: UpdatedDate
            );
            return Right<Error, DtoState>(state);
        }
    }

    }