using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata.Annotations;

internal sealed class AnnotationMetadataProvider<TMetadataAttribute, TMetadata>
    : IMetadataProvider<TMetadataAttribute, TMetadata>
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    private static readonly Lazy<AnnotationMetadataProvider<TMetadataAttribute, TMetadata>> LazyInstance = new(
        () => new AnnotationMetadataProvider<TMetadataAttribute, TMetadata>());

    private static readonly ConcurrentDictionary<CultureInfo, EphemeralValueCache<PropertyInfo, TMetadata>>
        PropertyMetadataCaches = new();

    private AnnotationMetadataProvider()
    {
    }

    public static AnnotationMetadataProvider<TMetadataAttribute, TMetadata> Instance => LazyInstance.Value;

    public bool TryGetMetadata(
        PropertyInfo property,
        [NotNullWhen(true)] out TMetadata? metadata,
        CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        var cache = PropertyMetadataCaches.GetOrAdd(culture, _ => new(new DeclaringTypePropertyComparer()));
        if (cache.TryGet(property, out metadata))
        {
            return true;
        }

        var metadataAttribute = property.GetCustomAttribute<TMetadataAttribute>(inherit: true);
        if (metadataAttribute is null)
        {
            return false;
        }

        metadataAttribute.Validate();

        metadata = cache.GetOrAdd(property, p =>
        {
            var allowedConversionAttributes = property.GetCustomAttributes<AllowUnitConversionAttribute>(inherit: true);
            return metadataAttribute.ToMetadata(
                property,
                metadataAttribute.BuildAllowedConversionsMetadata(allowedConversionAttributes, culture),
                culture: culture);
        });
        return true;
    }

    public void ValidateMetadata(PropertyInfo property)
    {
        if (!TryGetMetadata(property, out var metadata))
        {
            return;
        }

        metadata.Validate();
    }
}
