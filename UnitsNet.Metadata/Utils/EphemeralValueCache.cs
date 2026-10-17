using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UnitsNet.Metadata.Utils;

internal sealed class EphemeralValueCache<TKey, TItem> where TItem : class?
{
    private static readonly Lazy<EphemeralValueCache<TKey, TItem>> LazyGlobalInstance = new(() => new());

    private readonly ConcurrentDictionary<TKey, WeakReference<TItem>> _cache;

    public EphemeralValueCache(IEqualityComparer<TKey>? comparer = null)
    {
        _cache = new(comparer ?? EqualityComparer<TKey>.Default);
    }

    public static EphemeralValueCache<TKey, TItem> GlobalInstance => LazyGlobalInstance.Value;

    public bool TryGet(TKey key, [NotNullWhen(true)] out TItem? item)
    {
        item = default;

        if (!_cache.TryGetValue(key, out var reference) ||
            reference?.TryGetTarget(out item!) is not true)
        {
            _cache.TryRemove(key, out _);
            return false;
        }

        return true;
    }

    public TItem GetOrAdd(TKey key, Func<TKey, TItem> getter)
    {
        var reference = _cache.GetOrAdd(key, k =>
        {
            var item = getter(k);
            return new(item);
        });
        if (reference?.TryGetTarget(out var item) is true)
        {
            return item;
        }

        item = getter(key);
        if (item is not null)
        {
            _cache.AddOrUpdate(key, _ => new(item), (_, _) => new(item));
        }

        return item;
    }

    public void AddOrUpdate(TKey key, TItem item)
    {
        _cache.AddOrUpdate(key, _ => new(item), (_, _) => new(item));
    }

    public void Purge()
    {
        _cache.Clear();
    }
}
