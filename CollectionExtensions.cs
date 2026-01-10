using LanguageExt;

using static LanguageExt.Prelude;
namespace PCIShield.Domain.ModelsDto;

public static class CollectionExtensions
{
    public static Aff<int> ParallelAggregate<T>(
        this IEnumerable<T> source,
        int seed,
        Func<int, T, int> func) =>
        from items in Aff<Runtime, int>(() => source.AsParallel()
            .Aggregate(seed, func))
        select items;
}
