using System.Collections;
using System.Globalization;
using System.Linq.Expressions;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.Reflection;

namespace UnitsNet.Metadata.DynamicProxy;

/// <summary>
/// A builder of proxies that expose the quantity properties of a set of objects in other units.
/// </summary>
/// <typeparam name="TObject">The type of the objects to proxy</typeparam>
/// <typeparam name="TMetadataAttribute">The type of the metadata attribute</typeparam>
/// <typeparam name="TMetadata">The type of the metadata</typeparam>
public class DynamicProxyBuilder<TObject, TMetadataAttribute, TMetadata>
    where TObject : class
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    private readonly CultureInfo? _culture;
    private readonly IEnumerable _objects;
    private readonly DynamicProxyMetadataProvider<TMetadataAttribute, TMetadata> _metadataProvider;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="DynamicProxyBuilder{TObject, TMetadataAttribute, TMetadata}"/> class.
    /// </summary>
    /// <param name="objects">The objects to proxy</param>
    /// <param name="baseMetadataProvider">The provider of the underlying metadata</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public DynamicProxyBuilder(
        IEnumerable<TObject> objects,
        IMetadataProvider<TMetadataAttribute, TMetadata> baseMetadataProvider,
        CultureInfo? culture = null)
    {
        _culture = culture;
        _objects = objects ?? throw new ArgumentNullException(nameof(objects));
        _metadataProvider = new(baseMetadataProvider);
    }

    private DynamicProxyBuilder(
        IEnumerable objects,
        DynamicProxyMetadataProvider<TMetadataAttribute, TMetadata> dynamicMetadataProvider)
    {
        _objects = objects;
        _metadataProvider = dynamicMetadataProvider;
    }

    /// <summary>
    /// Converts the named property to the given unit.
    /// </summary>
    /// <param name="propertyName">The name of the property to convert</param>
    /// <param name="to">The unit to convert to</param>
    public DynamicProxyBuilder<TObject, TMetadataAttribute, TMetadata> WithConversion(string propertyName, Enum to)
    {
        var property = typeof(TObject).GetPropertyFlat(propertyName) ??
            throw new InvalidOperationException($"{propertyName} is not a property of {typeof(TObject).Name}");

        _metadataProvider.AddConversion(property, to, _culture);

        return this;
    }

    /// <summary>
    /// Converts the selected property to the given unit.
    /// </summary>
    /// <param name="propertySelectorExpression">An expression selecting the property to convert</param>
    /// <param name="to">The unit to convert to</param>
    public DynamicProxyBuilder<TObject, TMetadataAttribute, TMetadata> WithConversion(
        Expression<Func<TObject, QuantityValue>> propertySelectorExpression,
        Enum to)
    {
        var propertyName = propertySelectorExpression.ExtractPropertyName();
        var property = typeof(TObject).GetPropertyFlat(propertyName) ??
            throw new InvalidOperationException($"{propertyName} is not a property of {typeof(TObject).Name}");

        _metadataProvider.AddConversion(property, to, _culture);

        return this;
    }

    /// <summary>
    /// Creates a builder for a base type of the proxied objects.
    /// </summary>
    /// <typeparam name="TSuperObject">The base type to proxy as</typeparam>
    public DynamicProxyBuilder<TSuperObject, TMetadataAttribute, TMetadata> As<TSuperObject>()
        where TSuperObject : class
    {
        if (!typeof(TSuperObject).IsAssignableFrom(typeof(TObject)))
        {
            throw new InvalidOperationException(
                $"{nameof(TObject)} ({typeof(TObject).Name}) is not derived from {typeof(TSuperObject).Name}");
        }

        _metadataProvider.HoistMetadata<TSuperObject, TObject>(_culture);
        return new(_objects, _metadataProvider);
    }

    /// <summary>
    /// Validates the metadata and builds the proxies.
    /// </summary>
    public IDynamicProxyEnumerable<TObject, TMetadataAttribute, TMetadata> Build()
    {
        ((IMetadataProvider<TMetadataAttribute, TMetadata>)_metadataProvider).ValidateAllMetadata(typeof(TObject));
        return new DynamicQuantityEnumerable<TObject, TMetadataAttribute, TMetadata>(_objects, _metadataProvider);
    }
}
