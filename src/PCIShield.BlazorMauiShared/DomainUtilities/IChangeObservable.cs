using LanguageExt;

using PCIShieldLib.SharedKernel.Interfaces;

using static System.String;
namespace PCIShield.Domain.ModelsDto;
public record DtoState(bool IsDirty, bool HasChanges, DateTime? LastModified);
public interface IChangeObservable
{
    IObservable<Unit> Changes { get; }
    IObservable<PCIShield.Domain.ModelsDto.DtoState> StateChanges { get; }
    void NotifyChange();
}
public abstract class ErrorIdentifiableDtoBase : AuditBase, IErrorIdentity
{
    protected virtual (string Key, object? Value)? PrimaryId() => null;
    protected virtual (string Key, object? Value)? SecondaryId() => null;

    public IReadOnlyDictionary<string, object?> GetErrorIdentifiers()
    {
        var dict = new Dictionary<string, object?>(2);

        var p = PrimaryId();
        if (p.HasValue && !IsNullOrWhiteSpace(p.Value.Key))
            dict[p.Value.Key] = p.Value.Value;

        var s = SecondaryId();
        if (s.HasValue && !IsNullOrWhiteSpace(s.Value.Key))
            dict[s.Value.Key] = s.Value.Value;

        return dict;
    }
}
public interface IErrorIdentity
{
    IReadOnlyDictionary<string, object?> GetErrorIdentifiers();
}