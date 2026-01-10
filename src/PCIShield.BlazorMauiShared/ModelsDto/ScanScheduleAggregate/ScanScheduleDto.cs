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
    public static class ScanScheduleFactory
    {
        public static ScanScheduleDto CreateNewFromTemplate(ScanScheduleDto template)
        {
            var now = DateTime.UtcNow;

            var newScanSchedule = new ScanScheduleDto
            {
                ScanScheduleId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                IsEnabled = false,
                IsDeleted = false,
            };

            return newScanSchedule;
        }

        public static ScanScheduleDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new ScanScheduleDto
            {
                ScanScheduleId = Guid.Empty,
                TenantId = Guid.Empty,
                AssetId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsDeleted = false,
            };
        }
    }
    public static class ScanScheduleExtensions
    {
        public static ScanScheduleDto CloneAsNew(this ScanScheduleDto template)
        {
            return ScanScheduleFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class ScanScheduleDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<ScanScheduleDto>,ISnapshotable<ScanScheduleDto>,IChangeObservable
    {
        public Guid  ScanScheduleId { get;  set; }

        public Guid TenantId { get;  set; }

        public int ScanType { get;  set; }

        public string Frequency { get;  set; }

        public DateTime NextScanDate { get;  set; }

        public TimeSpan? BlackoutStart { get;  set; }

        public TimeSpan? BlackoutEnd { get;  set; }

        public bool IsEnabled { get;  set; }

        public DateTime CreatedAt { get;  set; }

        public Guid CreatedBy { get;  set; }

        public DateTime? UpdatedAt { get;  set; }

        public Guid? UpdatedBy { get;  set; }

        public bool IsDeleted { get;  set; }
        public int DaysSinceNextScanDate { get; set; }
        public bool IsEnabledFlag { get; set; }
        public bool IsScanTypeCritical { get; set; }

        public AssetDto Asset { get;  set; }

        public Guid AssetId { get;  set; }
        public ScanScheduleDto Clone()
        {
            return new ScanScheduleDto
            {
                ScanScheduleId = this.ScanScheduleId,
                TenantId = this.TenantId,
                ScanType = this.ScanType,
                Frequency = this.Frequency,
                NextScanDate = this.NextScanDate,
                BlackoutStart = this.BlackoutStart,
                BlackoutEnd = this.BlackoutEnd,
                IsEnabled = this.IsEnabled,
                CreatedAt = this.CreatedAt,
                CreatedBy = this.CreatedBy,
                UpdatedAt = this.UpdatedAt,
                UpdatedBy = this.UpdatedBy,
                IsDeleted = this.IsDeleted,
                AssetId = this.AssetId ,
                 Asset = (this.Asset!= null ? new  AssetDto
                {
                    AssetId = this.AssetId,
                    TenantId = this.Asset.TenantId,
                    AssetCode = this.Asset.AssetCode,
                    AssetName = this.Asset.AssetName,
                    AssetType = this.Asset.AssetType,
                    IPAddress = this.Asset.IPAddress,
                    Hostname = this.Asset.Hostname,
                    IsInCDE = this.Asset.IsInCDE,
                    NetworkZone = this.Asset.NetworkZone,
                    LastScanDate = this.Asset.LastScanDate,
                    CreatedAt = this.Asset.CreatedAt,
                    CreatedBy = this.Asset.CreatedBy,
                    UpdatedAt = this.Asset.UpdatedAt,
                    UpdatedBy = this.Asset.UpdatedBy,
                    IsDeleted = this.Asset.IsDeleted,
                }:null) ?? new AssetDto(),
            };
        }
        public ScanScheduleDto() { }
        public ScanScheduleDto(Guid scanScheduleId, Guid assetId, Guid tenantId, int scanType, string frequency, DateTime nextScanDate, bool isEnabled, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
                AuditEntityId = scanScheduleId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
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
        [JsonIgnore]
        private Option<ScanScheduleDto> _snapshot = None;

        public Option<ScanScheduleDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(ScanScheduleDto source)
        {
        ScanScheduleId = source.ScanScheduleId;
        TenantId = source.TenantId;
        ScanType = source.ScanType;
        Frequency = source.Frequency;
        NextScanDate = source.NextScanDate;
        BlackoutStart = source.BlackoutStart;
        BlackoutEnd = source.BlackoutEnd;
        IsEnabled = source.IsEnabled;
        CreatedAt = source.CreatedAt;
        CreatedBy = source.CreatedBy;
        UpdatedAt = source.UpdatedAt;
        UpdatedBy = source.UpdatedBy;
        IsDeleted = source.IsDeleted;
        AssetId = source.AssetId ;
            Asset = source.Asset?.Clone();
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
        public Either<Error, bool> Compare(ScanScheduleDto other)
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

        private Either<Error, bool> CompareBasicFields(ScanScheduleDto other)
        {
            bool same =
                (ScanScheduleId == other.ScanScheduleId) &&
                (TenantId == other.TenantId) &&
                (AssetId == other.AssetId) &&
                (ScanType == other.ScanType) &&
                StringComparer.OrdinalIgnoreCase.Equals(Frequency, other.Frequency) &&
                (NextScanDate == other.NextScanDate) &&
                (BlackoutStart == other.BlackoutStart) &&
                (BlackoutEnd == other.BlackoutEnd) &&
                (IsEnabled == other.IsEnabled) &&
                (CreatedAt == other.CreatedAt) &&
                (CreatedBy == other.CreatedBy) &&
                (UpdatedAt == other.UpdatedAt) &&
                (UpdatedBy == other.UpdatedBy) &&
                (IsDeleted == other.IsDeleted) ;
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
                LastModified: DateTime.UtcNow
            );
            return Right<Error, DtoState>(state);
        }
    }

    }