using LanguageExt;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Components;
using static LanguageExt.Prelude;
using static LanguageExt.Prelude;
using Unit = System.Reactive.Unit;
namespace PCIShield.BlazorAdmin.Client.Shared
{
    public static class PrimaryExtractor
    {
        public static Option<T> ExtractPrimary<T>(
            IEnumerable<T> items,
            Func<T, bool> isPrimaryPredicate,
            bool fallbackToFirst = false
        )
        {
            if (items == null) return None;
            var list = items.ToList();
            if (!list.Any()) return None;
            var found = list.FirstOrDefault(isPrimaryPredicate);
            if (found != null) return Some(found);
            if (fallbackToFirst)
            {
                return Some(list.First());
            }
            return None;
        }
    }
}
