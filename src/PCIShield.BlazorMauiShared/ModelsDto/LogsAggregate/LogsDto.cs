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
    public static class LogsFactory
    {
        public static LogsDto CreateNewFromTemplate(LogsDto template)
        {
            var now = DateTime.UtcNow;

            var newLogs = new LogsDto
            {
            };

            return newLogs;
        }

        public static LogsDto CreateNewEmpty()
        {
            var now = DateTime.UtcNow;
            return new LogsDto
            {
            };
        }
    }
    public static class LogsExtensions
    {
        public static LogsDto CloneAsNew(this LogsDto template)
        {
            return LogsFactory.CreateNewFromTemplate(template);
        }
    }

    public sealed class LogsDto : ErrorIdentifiableDtoBase, IModelDto,ITrackableEntity<LogsDto>,ISnapshotable<LogsDto>,IChangeObservable
    {
        public int  Id { get;  set; }

        public string? Message { get;  set; }

        public string? MessageTemplate { get;  set; }

        public string? Level { get;  set; }

        public string? Exception { get;  set; }

        public string? Properties { get;  set; }
        public LogsDto Clone()
        {
            return new LogsDto
            {
                Message = this.Message,
                MessageTemplate = this.MessageTemplate,
                Level = this.Level,
                Exception = this.Exception,
                Properties = this.Properties,
            };
        }
        public LogsDto() { }
        [JsonIgnore]
        private Option<LogsDto> _snapshot = None;

        public Option<LogsDto> CurrentSnapshot => _snapshot;

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

        private void CopyFrom(LogsDto source)
        {
        Message = source.Message;
        MessageTemplate = source.MessageTemplate;
        Level = source.Level;
        Exception = source.Exception;
        Properties = source.Properties;
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
        public Either<Error, bool> Compare(LogsDto other)
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

        private Either<Error, bool> CompareBasicFields(LogsDto other)
        {
            bool same =
                (Id == other.Id) &&
                StringComparer.OrdinalIgnoreCase.Equals(Message, other.Message) &&
                StringComparer.OrdinalIgnoreCase.Equals(MessageTemplate, other.MessageTemplate) &&
                StringComparer.OrdinalIgnoreCase.Equals(Level, other.Level) &&
                StringComparer.OrdinalIgnoreCase.Equals(Exception, other.Exception) &&
                StringComparer.OrdinalIgnoreCase.Equals(Properties, other.Properties) ;
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