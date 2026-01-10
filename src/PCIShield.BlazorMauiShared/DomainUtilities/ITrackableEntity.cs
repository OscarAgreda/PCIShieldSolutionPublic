using LanguageExt;
using LanguageExt.Common;
namespace PCIShield.Domain.ModelsDto;
public interface ITrackableEntity<T>
{
    Either<Error, bool> Compare(T other);
    Either<Error, DtoState> GetState();
}