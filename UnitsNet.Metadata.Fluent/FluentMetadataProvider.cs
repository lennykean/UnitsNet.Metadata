using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata.Fluent;

internal sealed class FluentMetadataProvider<TObject, TMetadataAttribute, TMetadata>
    : IMetadataProvider<TMetadataAttribute, TMetadata>
    where TObject : class
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    private readonly ConcurrentDictionary<PropertyInfo, TMetadata> _metadata = new();

    public bool TryGetMetadata(
        PropertyInfo property,
        [NotNullWhen(true)] out TMetadata? metadata,
        CultureInfo? culture = null)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        if (!_metadata.TryGetValue(property, out metadata))
        {
            return false;
        }

        if (culture is not null)
        {
            metadata = metadata.WithCulture(culture);
        }

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

    public void AddMetadata(PropertyInfo propertyInfo, TMetadata metadata)
    {
        _metadata.AddOrUpdate(propertyInfo, metadata, (_, _) => metadata);
    }
}
