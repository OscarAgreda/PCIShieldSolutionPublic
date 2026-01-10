using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
namespace PCIShield.Infrastructure.Helpers
{
    public static class UniversalOptionExtensions
    {
        public static T ValueOrDefault<T>(this T? nullable, T defaultValue) where T : struct
        {
            return nullable ?? defaultValue;
        }
        public static T ValueOrDefault<T>(this T? nullable, T defaultValue) where T : class
        {
            return nullable ?? defaultValue;
        }
        public static Guid ValueOrEmpty(this Guid? nullable)
        {
            return nullable ?? Guid.Empty;
        }
        public static DateTime ValueOrMin(this DateTime? nullable)
        {
            return nullable ?? DateTime.MinValue;
        }
        public static bool ValueOrFalse(this bool? nullable)
        {
            return nullable ?? false;
        }
        public static string ValueOrEmpty(this string? nullable)
        {
            return nullable ?? string.Empty;
        }
        public static T OptionValueOrDefault<T>(this Option<T> option, T defaultValue)
        {
            return option.Match(
                Some: value => value,
                None: () => defaultValue
            );
        }
        public static T ToOptionValueOrDefault<T>(this T? nullable, T defaultValue) where T : struct
        {
            return Optional(nullable).OptionValueOrDefault(defaultValue);
        }
        public static T ToOptionValueOrDefault<T>(this T? nullable, T defaultValue) where T : class
        {
            return Optional(nullable).OptionValueOrDefault(defaultValue);
        }
        public static Guid ToOptionValueOrEmpty(this Guid? nullable)
        {
            return Optional(nullable).OptionValueOrDefault(Guid.Empty);
        }
        public static DateTime ToOptionValueOrMin(this DateTime? nullable)
        {
            return Optional(nullable).OptionValueOrDefault(DateTime.MinValue);
        }
        public static bool ToOptionValueOrFalse(this bool? nullable)
        {
            return Optional(nullable).OptionValueOrDefault(false);
        }
        public static string ToOptionValueOrEmpty(this string? nullable)
        {
            return Optional(nullable).OptionValueOrDefault(string.Empty);
        }
    }
}
