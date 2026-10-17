using System.Linq.Expressions;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.Reflection;

namespace UnitsNet.Metadata.Fluent;

/// <summary>
/// A fluent builder of quantity metadata for the properties of an object type, with the default metadata types.
/// </summary>
/// <typeparam name="TObject">The type the metadata is built for</typeparam>
public class MetadataBuilder<TObject> : MetadataBuilder<TObject, QuantityAttribute, QuantityMetadata>
    where TObject : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MetadataBuilder{TObject}"/> class.
    /// </summary>
    public MetadataBuilder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetadataBuilder{TObject}"/> class.
    /// </summary>
    /// <param name="other">The builder to share registrations with</param>
    protected MetadataBuilder(MetadataBuilder<TObject> other)
    {
        MetadataAttributes = other.MetadataAttributes;
        AllowedConversionAttributes = other.AllowedConversionAttributes;
    }

    /// <summary>
    /// Registers a quantity for a property.
    /// </summary>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="asUnit">The unit the property value is expressed in</param>
    /// <param name="quantityType">The quantity type, or <see langword="null"/> to infer it from the unit</param>
    public QuantityMetadataBuilder<TObject> With(string propertyName, Enum asUnit, Type? quantityType = null)
    {
        var attribute = new QuantityAttribute(asUnit, quantityType);

        With(propertyName, attribute);

        return new(this, attribute);
    }

    /// <summary>
    /// Registers a quantity for a property.
    /// </summary>
    /// <param name="propertySelectorExpression">An expression selecting the property</param>
    /// <param name="asUnit">The unit the property value is expressed in</param>
    /// <param name="quantityType">The quantity type, or <see langword="null"/> to infer it from the unit</param>
    public QuantityMetadataBuilder<TObject> With(
        Expression<Func<TObject, QuantityValue>> propertySelectorExpression,
        Enum asUnit,
        Type? quantityType = null)
    {
        var propertyName = propertySelectorExpression.ExtractPropertyName();

        return With(propertyName, asUnit, quantityType);
    }
}
