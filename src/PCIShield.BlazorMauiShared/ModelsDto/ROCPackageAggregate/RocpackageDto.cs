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
    public static class ROCPackageFactory
    {
        public static ROCPackageDto CreateNewFromTemplate(ROCPackageDto template)
        {
            var now = DateTime.UtcNow;

            var newROCPackage = new ROCPackageDto
            {
                ROCPackageId = Guid.Empty,
                CreatedBy = template.CreatedBy,
                TenantId =  Guid.Empty,
                IsDeleted = false,
            };

            return newROCPackage;
        }

        public static ROCPackageDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new ROCPackageDto
            {
                ROCPackageId = Guid.Empty,
                TenantId = Guid.Empty,
                AssessmentId = Guid.Empty,
                CreatedBy = Guid.Empty,
                UpdatedBy = Guid.Empty,
                IsDeleted = false,
            };
        }
    }
    public static class ROCPackageExtensions
    {
        public static ROCPackageDto CloneAsNew(this ROCPackageDto template)
        {
            return ROCPackageFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class ROCPackageDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<ROCPackageDto>,ISnapshotable<ROCPackageDto>,IChangeObservable
    {
        public Guid  ROCPackageId { get;  set; }

        public Guid TenantId { get;  set; }

        public string PackageVersion { get;  set; }

        public DateTime GeneratedDate { get;  set; }

        public string? QSAName { get;  set; }

        public string? QSACompany { get;  set; }

        public DateTime? SignatureDate { get;  set; }

        public string? AOCNumber { get;  set; }

        public int Rank { get;  set; }

        public DateTime CreatedAt { get;  set; }

        public Guid CreatedBy { get;  set; }

        public DateTime? UpdatedAt { get;  set; }

        public Guid? UpdatedBy { get;  set; }

        public bool IsDeleted { get;  set; }
        public int DaysSinceGeneratedDate { get; set; }
        public bool IsDeletedFlag { get; set; }
        public bool IsRankCritical { get; set; }

        public AssessmentDto Assessment { get;  set; }

        public Guid AssessmentId { get;  set; }
        public ROCPackageDto Clone()
        {
            return new ROCPackageDto
            {
                ROCPackageId = this.ROCPackageId,
                TenantId = this.TenantId,
                PackageVersion = this.PackageVersion,
                GeneratedDate = this.GeneratedDate,
                QSAName = this.QSAName,
                QSACompany = this.QSACompany,
                SignatureDate = this.SignatureDate,
                AOCNumber = this.AOCNumber,
                Rank = this.Rank,
                CreatedAt = this.CreatedAt,
                CreatedBy = this.CreatedBy,
                UpdatedAt = this.UpdatedAt,
                UpdatedBy = this.UpdatedBy,
                IsDeleted = this.IsDeleted,
                AssessmentId = this.AssessmentId ,
                 Assessment = (this.Assessment!= null ? new  AssessmentDto
                {
                    AssessmentId = this.AssessmentId,
                    TenantId = this.Assessment.TenantId,
                    AssessmentCode = this.Assessment.AssessmentCode,
                    AssessmentType = this.Assessment.AssessmentType,
                    AssessmentPeriod = this.Assessment.AssessmentPeriod,
                    StartDate = this.Assessment.StartDate,
                    EndDate = this.Assessment.EndDate,
                    CompletionDate = this.Assessment.CompletionDate,
                    Rank = this.Assessment.Rank,
                    ComplianceScore = this.Assessment.ComplianceScore,
                    QSAReviewRequired = this.Assessment.QSAReviewRequired,
                    CreatedAt = this.Assessment.CreatedAt,
                    CreatedBy = this.Assessment.CreatedBy,
                    UpdatedAt = this.Assessment.UpdatedAt,
                    UpdatedBy = this.Assessment.UpdatedBy,
                    IsDeleted = this.Assessment.IsDeleted,
                }:null) ?? new AssessmentDto(),
            };
        }
        public ROCPackageDto() { }
        public ROCPackageDto(Guid rocpackageId, Guid assessmentId, Guid tenantId, string packageVersion, DateTime generatedDate, int rank, DateTime createdAt, Guid createdBy, bool isDeleted)
        {
                AuditEntityId = rocpackageId.ToString();
                AuditId = Guid.CreateVersion7().ToString();
                AuditEntityType = GetType().Name;
                this.ROCPackageId = Guard.Against.Default(rocpackageId, nameof(rocpackageId));
                this.AssessmentId = Guard.Against.Default(assessmentId, nameof(assessmentId));
                this.TenantId = Guard.Against.Default(tenantId, nameof(tenantId));
                this.PackageVersion = Guard.Against.NullOrEmpty(packageVersion, nameof(packageVersion));
                this.GeneratedDate = Guard.Against.OutOfSQLDateRange(generatedDate, nameof(generatedDate));
                this.Rank = Guard.Against.Negative(rank, nameof(rank));
                this.CreatedAt = Guard.Against.OutOfSQLDateRange(createdAt, nameof(createdAt));
                this.CreatedBy = Guard.Against.Default(createdBy, nameof(createdBy));
                this.IsDeleted = Guard.Against.Null(isDeleted, nameof(isDeleted));
        }
        [JsonIgnore]
        private Option<ROCPackageDto> _snapshot = None;

        public Option<ROCPackageDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(ROCPackageDto source)
        {
        ROCPackageId = source.ROCPackageId;
        TenantId = source.TenantId;
        PackageVersion = source.PackageVersion;
        GeneratedDate = source.GeneratedDate;
        QSAName = source.QSAName;
        QSACompany = source.QSACompany;
        SignatureDate = source.SignatureDate;
        AOCNumber = source.AOCNumber;
        Rank = source.Rank;
        CreatedAt = source.CreatedAt;
        CreatedBy = source.CreatedBy;
        UpdatedAt = source.UpdatedAt;
        UpdatedBy = source.UpdatedBy;
        IsDeleted = source.IsDeleted;
        AssessmentId = source.AssessmentId ;
            Assessment = source.Assessment?.Clone();
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
        public Either<Error, bool> Compare(ROCPackageDto other)
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

        private Either<Error, bool> CompareBasicFields(ROCPackageDto other)
        {
            bool same =
                (ROCPackageId == other.ROCPackageId) &&
                (TenantId == other.TenantId) &&
                (AssessmentId == other.AssessmentId) &&
                StringComparer.OrdinalIgnoreCase.Equals(PackageVersion, other.PackageVersion) &&
                (GeneratedDate == other.GeneratedDate) &&
                StringComparer.OrdinalIgnoreCase.Equals(QSAName, other.QSAName) &&
                StringComparer.OrdinalIgnoreCase.Equals(QSACompany, other.QSACompany) &&
                (SignatureDate == other.SignatureDate) &&
                StringComparer.OrdinalIgnoreCase.Equals(AOCNumber, other.AOCNumber) &&
                (Rank == other.Rank) &&
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