using FluentValidation;
using FluentValidation.Results;

using LanguageExt;
using LanguageExt.Common;

namespace PCIShield.BlazorAdmin.Client.Shared.Validation;

public sealed class SaveEntityStrategy<T>
{
    private readonly IValidator<T> _validator;
    private readonly Func<T, CancellationToken, Task<Either<Error, Unit>>> _create;
    private readonly Func<T, CancellationToken, Task<Either<Error, Unit>>> _update;
    private readonly Func<T, bool> _isNew;
    private readonly Action<T>? _preSave;

    public SaveEntityStrategy(
        IValidator<T> validator,
        Func<T, CancellationToken, Task<Either<Error, Unit>>> create,
        Func<T, CancellationToken, Task<Either<Error, Unit>>> update,
        Func<T, bool> isNew,
        Action<T>? preSave = null)
    {
        _validator = validator;
        _create = create;
        _update = update;
        _isNew = isNew;
        _preSave = preSave;
    }

    public async Task<Either<Error, Unit>> SaveAsync(T entity, CancellationToken ct = default)
    {
        _preSave?.Invoke(entity);

        var vr = await _validator.ValidateAsync(entity, ct);
        if (!vr.IsValid)
        {
            var detailed = ValidationFormatting.ToDetailedString(vr, entity);
            return LanguageExt.Prelude.Left<Error, Unit>(Error.New(detailed));
        }

        return _isNew(entity)
            ? await _create(entity, ct)
            : await _update(entity, ct);
    }
}
