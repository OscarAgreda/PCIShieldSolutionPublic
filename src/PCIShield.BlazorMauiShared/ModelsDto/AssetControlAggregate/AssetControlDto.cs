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
    public static class AssetControlFactory
    {
        public static AssetControlDto CreateNewFromTemplate(AssetControlDto template)
        {
            var now = DateTime.UtcNow;

            var newAssetControl = new AssetControlDto
            {
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                IsApplicable = false,
                IsDeleted = false,
            };

            return newAssetControl;
        }

        public static AssetControlDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new AssetControlDto
            {
                AssetId = Guid.Empty,
                ControlId = Guid.Empty,
                TenantId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsDeleted = false,
            };
        }
    }
    public static class AssetControlExtensions
    {
        public static AssetControlDto CloneAsNew(this AssetControlDto template)
        {
            return AssetControlFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class AssetControlDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<AssetControlDto>,ISnapshotable<AssetControlDto>,IChangeObservable
    {
        public int  RowId { get;  set; }

        public Guid TenantId { get;  set; }

        public bool IsApplicable { get;  set; }

        public string? CustomizedApproach { get;  set; }

        public DateTime CreatedAt { get;  set; }

        public Guid CreatedBy { get;  set; }

        public DateTime? UpdatedAt { get;  set; }

        public Guid? UpdatedBy { get;  set; }

        public bool IsDeleted { get;  set; }

        public AssetDto Asset { get;  set; }

        public Guid AssetId { get;  set; }

        public ControlDto Control { get;  set; }

        public Guid ControlId { get;  set; }
        public AssetControlDto Clone()
        {
            return new AssetControlDto
            {
                TenantId = this.TenantId,
                IsApplicable = this.IsApplicable,
                CustomizedApproach = this.CustomizedApproach,
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
                ControlId = this.ControlId ,
                 Control = (this.Control!= null ? new  ControlDto
                {
                    ControlId = this.ControlId,
                    TenantId = this.Control.TenantId,
                    ControlCode = this.Control.ControlCode,
                    RequirementNumber = this.Control.RequirementNumber,
                    ControlTitle = this.Control.ControlTitle,
                    ControlDescription = this.Control.ControlDescription,
                    TestingGuidance = this.Control.TestingGuidance,
                    FrequencyDays = this.Control.FrequencyDays,
                    IsMandatory = this.Control.IsMandatory,
                    EffectiveDate = this.Control.EffectiveDate,
                    CreatedAt = this.Control.CreatedAt,
                    CreatedBy = this.Control.CreatedBy,
                    UpdatedAt = this.Control.UpdatedAt,
                    UpdatedBy = this.Control.UpdatedBy,
                    IsDeleted = this.Control.IsDeleted,
                }:null) ?? new ControlDto(),
            };
        }
        public AssetControlDto() { }
        public AssetControlDto(Guid assetId, Guid controlId, Guid tenantId, bool isApplicable, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
                this.AssetId = Guard.Against.Default(assetId, nameof(assetId));
                this.ControlId = Guard.Against.Default(controlId, nameof(controlId));
                this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
                this.IsApplicable = Guard.Against.Null(isApplicable, nameof(isApplicable));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
                this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
        }
        [JsonIgnore]
        private Option<AssetControlDto> _snapshot = None;

        public Option<AssetControlDto> CurrentSnapshot => _snapshot;

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
                    NotifyChange();
                    return Right<Error, Unit>(LanguageExt.Unit.Default);
                },
                None: () => Left<Error, Unit>(Error.New("No snapshot exists to restore"))
            );

        private void CopyFrom(AssetControlDto source)
        {
        TenantId = source.TenantId;
        IsApplicable = source.IsApplicable;
        CustomizedApproach = source.CustomizedApproach;
        CreatedAt = source.CreatedAt;
        CreatedBy = source.CreatedBy;
        UpdatedAt = source.UpdatedAt;
        UpdatedBy = source.UpdatedBy;
        IsDeleted = source.IsDeleted;
        AssetId = source.AssetId ;
            Asset = source.Asset?.Clone();
        ControlId = source.ControlId ;
            Control = source.Control?.Clone();
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
        public Either<Error, bool> Compare(AssetControlDto other)
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

        private Either<Error, bool> CompareBasicFields(AssetControlDto other)
        {
            bool same =
                (RowId == other.RowId) &&
                (AssetId == other.AssetId) &&
                (ControlId == other.ControlId) &&
                (TenantId == other.TenantId) &&
                (IsApplicable == other.IsApplicable) &&
                StringComparer.OrdinalIgnoreCase.Equals(CustomizedApproach, other.CustomizedApproach) &&
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