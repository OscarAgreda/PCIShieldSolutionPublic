using LanguageExt;
using LanguageExt.Common;
namespace PCIShield.Domain.ModelsDto;
public interface ISnapshotable<T>
{
    void TakeSnapshot();
    bool IsDirty { get; }
    Option<T> CurrentSnapshot { get; }
    Either<Error, Unit> RestoreSnapshot();
}