using System.Globalization;

using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata.DynamicProxy;

/// <summary>
/// A sequence of dynamic proxies that provides their quantity metadata.
/// </summary>
/// <typeparam name="TObject">The type of the proxied objects</typeparam>
/// <typeparam name="TMetadataAttribute">The type of the metadata attribute</typeparam>
/// <typeparam name="TMetadata">The type of the metadata</typeparam>
public interface IDynamicProxyEnumerable<TObject, TMetadataAttribute, TMetadata> :
    IEnumerable<TObject>,
    IMetadataProvider<TMetadataAttribute, TMetadata>
    where TObject : class
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    /// <summary>
    /// Gets the metadata of the proxied objects.
    /// </summary>
    /// <param name="culture">The culture to use, or <see langword="null"/> for the current culture</param>
    IReadOnlyDictionary<string, TMetadata> GetObjectMetadata(CultureInfo? culture = null);
}
