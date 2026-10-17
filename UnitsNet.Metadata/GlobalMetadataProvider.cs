using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata;

/// <summary>
/// The metadata provider that combines every registered provider.
/// </summary>
/// <typeparam name="TMetadataAttribute">The metadata attribute type</typeparam>
/// <typeparam name="TMetadata">The metadata type</typeparam>
public sealed class GlobalMetadataProvider<TMetadataAttribute, TMetadata>
    : IMetadataProvider<TMetadataAttribute, TMetadata>
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    private static readonly Lazy<GlobalMetadataProvider<TMetadataAttribute, TMetadata>> LazyInstance = new(() => new());

    private readonly ConcurrentDictionary<
        Type,
        (IMetadataProvider<TMetadataAttribute, TMetadata> provider, int priority)> _providers = new();

    static GlobalMetadataProvider()
    {
        Instance.RegisterProvider(AnnotationMetadataProvider<TMetadataAttribute, TMetadata>.Instance, 2);
    }

    /// <summary>
    /// Gets the shared provider instance.
    /// </summary>
    public static GlobalMetadataProvider<TMetadataAttribute, TMetadata> Instance => LazyInstance.Value;

    /// <summary>
    /// Registers a metadata provider, replacing any registered provider of the same type.
    /// </summary>
    /// <param name="provider">The provider to register</param>
    /// <param name="priority">The priority of the provider; lower values take precedence</param>
    public void RegisterProvider(IMetadataProvider<TMetadataAttribute, TMetadata> provider, int priority = 0)
    {
        _providers.AddOrUpdate(provider.GetType(), (provider, priority), (_, _) => (provider, priority));
    }

    /// <summary>
    /// Removes all registered providers.
    /// </summary>
    public void ClearProviders()
    {
        _providers.Clear();
    }

    /// <inheritdoc />
    public bool TryGetMetadata(
        PropertyInfo property,
        [NotNullWhen(true)] out TMetadata? metadata,
        CultureInfo? culture = null)
    {
        foreach (var provider in _providers.Values.OrderBy(p => p.priority).Select(p => p.provider))
        {
            if (provider.TryGetMetadata(property, out metadata, culture))
            {
                return true;
            }
        }

        metadata = default;
        return false;
    }

    /// <inheritdoc />
    public void ValidateMetadata(PropertyInfo property)
    {
        foreach (var provider in _providers.Values.OrderBy(p => p.priority).Select(p => p.provider))
        {
            provider.ValidateMetadata(property);
        }
    }
}
