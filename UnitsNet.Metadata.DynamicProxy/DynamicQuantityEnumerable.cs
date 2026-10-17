using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using Castle.DynamicProxy;

using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata.DynamicProxy;

internal sealed class DynamicQuantityEnumerable<TObject, TMetadataAttribute, TMetadata>
    : IDynamicProxyEnumerable<TObject, TMetadataAttribute, TMetadata>
    where TObject : class
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    private readonly ConcurrentDictionary<TObject, TObject> _proxyCache = new();

    private readonly IEnumerable _objects;
    private readonly DynamicProxyMetadataProvider<TMetadataAttribute, TMetadata> _dynamicMetadataProvider;

    public DynamicQuantityEnumerable(
        IEnumerable objects,
        DynamicProxyMetadataProvider<TMetadataAttribute, TMetadata> dynamicMetadataProvider)
    {
        _objects = objects ?? throw new ArgumentNullException(nameof(objects));
        _dynamicMetadataProvider = dynamicMetadataProvider ??
            throw new ArgumentNullException(nameof(dynamicMetadataProvider));
    }

    public IEnumerator<TObject> GetEnumerator()
    {
        var options = new ProxyGenerationOptions();
        options.AddMixinInstance(_dynamicMetadataProvider);

        var proxyGenerator = new ProxyGenerator();
        var interceptor = new DynamicQuantityInterceptor<TObject, TMetadataAttribute, TMetadata>(
            _dynamicMetadataProvider);

        Func<object, TObject> createProxy = typeof(TObject).IsInterface
            ? proxyObj => (TObject)proxyGenerator.CreateInterfaceProxyWithTarget(
                typeof(TObject),
                proxyObj,
                options,
                interceptor)
            : proxyObj => (TObject)proxyGenerator.CreateClassProxyWithTarget(
                typeof(TObject),
                proxyObj,
                options,
                interceptor);

        foreach (TObject obj in _objects)
        {
            yield return _proxyCache.GetOrAdd(obj, _ => createProxy(obj));
        }
    }

    public bool TryGetMetadata(
        PropertyInfo property,
        [NotNullWhen(true)] out TMetadata? metadata,
        CultureInfo? culture = null)
    {
        return _dynamicMetadataProvider.TryGetMetadata(property, out metadata, culture);
    }

    public void ValidateMetadata(PropertyInfo property)
    {
        _dynamicMetadataProvider.ValidateMetadata(property);
    }

    public IReadOnlyDictionary<string, TMetadata> GetObjectMetadata(CultureInfo? culture)
    {
        var dynamicMetadataProvider = _dynamicMetadataProvider as IMetadataProvider<TMetadataAttribute, TMetadata>;

        dynamicMetadataProvider.ValidateAllMetadata(typeof(TObject));

        return new MetadataDictionary<TMetadata>(dynamicMetadataProvider.GetMetadata(typeof(TObject), culture));
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
