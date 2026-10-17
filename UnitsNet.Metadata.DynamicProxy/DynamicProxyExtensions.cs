using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata.DynamicProxy;

/// <summary>
/// Extension methods for creating dynamic proxies of objects.
/// </summary>
public static class DynamicProxyExtensions
{
    /// <summary>
    /// Creates a builder for dynamic proxies of the objects.
    /// </summary>
    /// <typeparam name="TObject">The type of the objects to proxy</typeparam>
    /// <typeparam name="TMetadataAttribute">The type of the metadata attribute</typeparam>
    /// <typeparam name="TMetadata">The type of the metadata</typeparam>
    /// <param name="objects">The objects to proxy</param>
    public static DynamicProxyBuilder<TObject, TMetadataAttribute, TMetadata>
        AsDynamicProxies<TObject, TMetadataAttribute, TMetadata>(this IEnumerable<TObject> objects)
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
        where TObject : class
    {
        var baseMetadataProvider =
            objects as IMetadataProvider<TMetadataAttribute, TMetadata> ??
            GlobalMetadataProvider<TMetadataAttribute, TMetadata>.Instance;

        return new(objects, baseMetadataProvider);
    }

    /// <summary>
    /// Creates a builder for dynamic proxies of the objects using the default quantity metadata.
    /// </summary>
    /// <typeparam name="TObject">The type of the objects to proxy</typeparam>
    /// <param name="objects">The objects to proxy</param>
    public static DynamicProxyBuilder<TObject, QuantityAttribute, QuantityMetadata> AsDynamicProxies<TObject>(
        this IEnumerable<TObject> objects)
        where TObject : class
    {
        return objects.AsDynamicProxies<TObject, QuantityAttribute, QuantityMetadata>();
    }
}
