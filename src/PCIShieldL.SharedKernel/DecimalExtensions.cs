using System;
using LanguageExt;
using static LanguageExt.Prelude;
namespace PCIShieldLib.SharedKernel;
public static class DecimalExtensions
{
    public static Option<decimal> ToOption(this decimal? value) => 
        value.HasValue ? Some(value.Value) : None;
    public static decimal? ToOptionDecimalNullable(this decimal? value) =>
        value.HasValue ? value.Value : null;
    public static Guid ToOptionGuid(this Guid? nullable) =>
        Optional(nullable).MatchUnsafe(
            Some: value => value,
            None: () => Guid.Empty);
    public static T ToNonNullable<T>(this T? nullable) where T : struct =>
        Optional(nullable).MatchUnsafe(
            Some: value => value,
            None: () => default);
    public static string ToNonNullableString(this string? nullable) =>
        Optional(nullable).MatchUnsafe(
            Some: value => value,
            None: () => string.Empty);
}