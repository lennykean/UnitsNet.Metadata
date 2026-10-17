using System.Collections.Concurrent;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.Reflection;

namespace UnitsNet.Metadata.Fluent;

/// <summary>
/// A fluent builder of quantity metadata for the properties of an object type.
/// </summary>
/// <typeparam name="TObject">The type the metadata is built for</typeparam>
/// <typeparam name="TMetadataAttribute">The metadata attribute type</typeparam>
/// <typeparam name="TMetadata">The metadata type</typeparam>
public class MetadataBuilder<TObject, TMetadataAttribute, TMetadata> : MetadataBuilder
    where TObject : class
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MetadataBuilder{TObject, TMetadataAttribute, TMetadata}"/> class.
    /// </summary>
    public MetadataBuilder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetadataBuilder{TObject, TMetadataAttribute, TMetadata}"/> class.
    /// </summary>
    /// <param name="other">The builder to share registrations with</param>
    protected MetadataBuilder(MetadataBuilder<TObject, TMetadataAttribute, TMetadata> other)
    {
        MetadataAttributes = other.MetadataAttributes;
        AllowedConversionAttributes = other.AllowedConversionAttributes;
    }

    /// <summary>
    /// Gets or sets the metadata attributes registered per property.
    /// </summary>
    protected ConcurrentDictionary<PropertyInfo, TMetadataAttribute> MetadataAttributes { get; set; } = new();

    /// <summary>
    /// Gets or sets the allowed unit conversion attributes registered per metadata attribute.
    /// </summary>
    protected ConcurrentDictionary<TMetadataAttribute, ConcurrentBag<AllowUnitConversionAttribute>>
        AllowedConversionAttributes
    { get; set; } = new();

    /// <summary>
    /// Registers a metadata attribute for a property.
    /// </summary>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="attribute">The metadata attribute to apply</param>
    public QuantityMetadataBuilder<TObject, TMetadataAttribute, TMetadata> With(
        string propertyName,
        TMetadataAttribute attribute)
    {
        var property = typeof(TObject).GetPropertyFlat(propertyName) ??
            throw new InvalidOperationException($"{propertyName} is not a property of {typeof(TObject).Name}");

        MetadataAttributes.AddOrUpdate(property, attribute, (_, _) => attribute);

        return new(this, attribute);
    }

    /// <summary>
    /// Registers a metadata attribute for a property.
    /// </summary>
    /// <param name="propertySelectorExpression">An expression selecting the property</param>
    /// <param name="attribute">The metadata attribute to apply</param>
    public QuantityMetadataBuilder<TObject, TMetadataAttribute, TMetadata> With(
        Expression<Func<TObject, QuantityValue>> propertySelectorExpression,
        TMetadataAttribute attribute)
    {
        var propertyName = propertySelectorExpression.ExtractPropertyName();

        return With(propertyName, attribute);
    }

    /// <summary>
    /// Builds the metadata provider for the registered properties.
    /// </summary>
    /// <param name="global">Whether to register the provider globally</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public IMetadataProvider<TMetadataAttribute, TMetadata> Build(bool global = true, CultureInfo? culture = null)
    {
        var provider = new FluentMetadataProvider<TObject, TMetadataAttribute, TMetadata>();

        foreach (var (property, metadataAttribute) in MetadataAttributes)
        {
            var allowedConversionAttributes = AllowedConversionAttributes.TryGetValue(
                metadataAttribute,
                out var attributes)
                ? attributes
                : Enumerable.Empty<AllowUnitConversionAttribute>();

            var allowedConversions =
                metadataAttribute.BuildAllowedConversionsMetadata(allowedConversionAttributes, culture);

            provider.AddMetadata(
                property,
                metadataAttribute.ToMetadata(property, allowedConversions, culture: culture));
        }

        if (global)
        {
            GlobalMetadataProvider<TMetadataAttribute, TMetadata>.Instance.RegisterProvider(provider, priority: 1);
        }

        return provider;
    }
}
