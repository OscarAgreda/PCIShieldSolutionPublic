using System.Collections.Concurrent;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
namespace PCIShield.Domain.ModelsDto;
public static class CollectionExtensionsV2
{
    private static readonly ConcurrentDictionary<string, int> HashCache = new();
    private const int ParallelThreshold = 1000;
    public static Either<Error, bool> CompareCollection<T>(List<T> current, List<T> other)
    {
        if (ReferenceEquals(current, other))
            return Right<Error, bool>(true);
        if (current.Count != other.Count)
            return Right<Error, bool>(false);
        var key = $"{current.GetHashCode()}_{current.Count}_{other.GetHashCode()}_{other.Count}";
        if (HashCache.TryGetValue(key, out var cached))
            return Right<Error, bool>(cached == 1);
        return from currentHash in GetCachedHash(current)
               from otherHash in GetCachedHash(other)
               from eqResult in ValidateCollectionEquality(current, other, currentHash, otherHash)
               from final in AddToCache(key, eqResult)
               select final;
    }
    private static Either<Error, int> GetCachedHash<T>(List<T> items)
    {
        var cacheKey = $"{items.GetHashCode()}_{items.Count}";
        if (HashCache.TryGetValue(cacheKey, out var cachedValue))
            return Right<Error, int>(cachedValue);
        int newHash = (items.Count > ParallelThreshold)
            ? items.AsParallel().Aggregate(0, (acc, item) => acc ^ (item?.GetHashCode() ?? 0))
            : items.Aggregate(0, (acc, item) => acc ^ (item?.GetHashCode() ?? 0));
        HashCache[cacheKey] = newHash;
        return Right<Error, int>(newHash);
    }
    private static Either<Error, bool> ValidateCollectionEquality<T>(
        List<T> current,
        List<T> other,
        int currentHash,
        int otherHash
    )
    {
        if (currentHash != otherHash)
            return FullIterationCheck(current, other);
        if (current.Count > 0)
        {
            if (!Equals(current[0], other[0]))
                return Left<Error, bool>(Error.New($"Mismatch at index 0"));
        }
        if (current.Count > 1)
        {
            if (!Equals(current[^1], other[^1]))
                return Left<Error, bool>(Error.New($"Mismatch at last index"));
        }
        if (current.Count > 2)
        {
            int midIndex = current.Count / 2;
            if (!Equals(current[midIndex], other[midIndex]))
                return Left<Error, bool>(Error.New($"Mismatch at middle index {midIndex}"));
        }
        return FullIterationCheck(current, other);
    }
    private static Either<Error, bool> FullIterationCheck<T>(List<T> current, List<T> other)
    {
        for (int i = 0; i < current.Count; i++)
        {
            if (!Equals(current[i], other[i]))
                return Left<Error, bool>(Error.New($"Mismatch at index {i}"));
        }
        return Right<Error, bool>(true);
    }
    private static Either<Error, bool> AddToCache(string key, bool result)
    {
        bool inserted = HashCache.TryAdd(key, result ? 1 : 0);
        return inserted
            ? Right<Error, bool>(result)
            : Left<Error, bool>(Error.New($"Could not add result to HashCache: {key}"));
    }
}
public static class CollectionExtensionsV1
{
    private static readonly ConcurrentDictionary<string, int> HashCache = new();
    private const int MAX_CACHE_SIZE = 10000;
    private const int ParallelThreshold = 1000;
    public static Either<Error, bool> CompareCollection<T>(List<T> current, List<T> other)
    {
        if (ReferenceEquals(current, other))
            return Right<Error, bool>(true);
        if (current.Count != other.Count)
            return Right<Error, bool>(false);
        var key = $"{current.GetHashCode()}_{current.Count}_{other.GetHashCode()}_{other.Count}";
        if (HashCache.TryGetValue(key, out var cachedResult))
            return Right<Error, bool>(cachedResult == 1);
        return from currentHash in GetCachedHash(current)
               from otherHash in GetCachedHash(other)
               from eqResult in ValidateCollectionEquality(current, other, currentHash, otherHash)
               from final in AddToCache(key, eqResult)
               select final;
    }
    private static Either<Error, int> GetCachedHash<T>(List<T> items)
    {
        var cacheKey = $"{items.GetHashCode()}_{items.Count}";
        if (HashCache.TryGetValue(cacheKey, out var existingHash))
            return Right<Error, int>(existingHash);
        int newHash = 0;
        if (items.Count > ParallelThreshold)
        {
            newHash = items.AsParallel().Aggregate(0, (acc, item) => acc ^ (item?.GetHashCode() ?? 0));
        }
        else
        {
            foreach (var itm in items)
                newHash ^= (itm?.GetHashCode() ?? 0);
        }
        CleanUpCacheIfNeeded();
        HashCache[cacheKey] = newHash;
        return Right<Error, int>(newHash);
    }
    private static Either<Error, bool> ValidateCollectionEquality<T>(
        List<T> current,
        List<T> other,
        int currentHash,
        int otherHash
    )
    {
        for (int i = 0; i < current.Count; i++)
        {
            if (!Equals(current[i], other[i]))
                return Right<Error, bool>(false);
        }
        return Right<Error, bool>(true);
    }
    private static Either<Error, bool> AddToCache(string key, bool result)
    {
        int value = result ? 1 : 0;
        CleanUpCacheIfNeeded();
        HashCache.TryAdd(key, value);
        return Right<Error, bool>(result);
    }
    private static void CleanUpCacheIfNeeded()
    {
        if (HashCache.Count > MAX_CACHE_SIZE)
        {
            int removeCount = HashCache.Count / 2;
            var keysToRemove = HashCache.Keys.Take(removeCount).ToList();
            foreach (var k in keysToRemove)
            {
                HashCache.TryRemove(k, out _);
            }
        }
    }
}
