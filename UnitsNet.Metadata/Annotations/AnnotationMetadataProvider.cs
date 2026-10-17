using System;
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

    private static readonly Lazy<EphemeralValueCache<PropertyInfo, TMetadata>> LazyPropertyMetadataCache = new(
        () => new(new DeclaringTypePropertyComparer()));

    private AnnotationMetadataProvider()
    {
    }

    public static AnnotationMetadataProvider<TMetadataAttribute, TMetadata> Instance => LazyInstance.Value;

    public bool TryGetMetadata(
        PropertyInfo property,
        [NotNullWhen(true)] out TMetadata? metadata,
        CultureInfo? culture = null)
    {
        if (LazyPropertyMetadataCache.Value.TryGet(property, out metadata))
        {
            return true;
        }

        var metadataAttribute = property.GetCustomAttribute<TMetadataAttribute>(inherit: true);
        if (metadataAttribute is null)
        {
            return false;
        }

        metadataAttribute.Validate();

        metadata = LazyPropertyMetadataCache.Value.GetOrAdd(property, p =>
        {
            var allowedConversionAttributes = property.GetCustomAttributes<AllowUnitConversionAttribute>(inherit: true);
            return metadataAttribute.ToMetadata(
                property,
                metadataAttribute.BuildAllowedConversionsMetadata(allowedConversionAttributes, culture));
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
