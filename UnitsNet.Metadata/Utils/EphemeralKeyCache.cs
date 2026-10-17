using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace UnitsNet.Metadata.Utils;

internal sealed class EphemeralKeyCache<TKey, TItem> where TKey : class where TItem : class?
{
    private static readonly Lazy<EphemeralKeyCache<TKey, TItem>> LazyGlobalInstance = new(() => new());

    private readonly ConditionalWeakTable<TKey, TItem> _cache;

    public EphemeralKeyCache(IEqualityComparer<TKey>? comparer = null)
    {
        _cache = new();
    }

    public static EphemeralKeyCache<TKey, TItem> GlobalInstance => LazyGlobalInstance.Value;

    public bool TryGet(TKey key, [MaybeNullWhen(true)] out TItem? item)
    {
        if (!_cache.TryGetValue(key, out item))
        {
            return false;
        }

        return true;
    }

    public TItem GetOrAdd(TKey key, Func<TKey, TItem> getter)
    {
        return _cache.GetValue(key, k => getter(k));
    }

    public void AddOrUpdate(TKey key, TItem item)
    {
        _cache.AddOrUpdate(key, item);
    }

    public void Purge()
    {
        _cache.Clear();
    }
}
