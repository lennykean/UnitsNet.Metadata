using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata.Fluent;

/// <summary>
/// A fluent builder of the metadata of a quantity property.
/// </summary>
/// <typeparam name="TObject">The type that owns the property</typeparam>
public class QuantityMetadataBuilder<TObject> : MetadataBuilder<TObject>
    where TObject : class
{
    private readonly QuantityAttribute _attribute;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuantityMetadataBuilder{TObject}"/> class.
    /// </summary>
    /// <param name="inner">The builder to continue from</param>
    /// <param name="attribute">The quantity attribute being configured</param>
    public QuantityMetadataBuilder(MetadataBuilder<TObject> inner, QuantityAttribute attribute) : base(inner)
    {
        _attribute = attribute;
    }

    /// <summary>
    /// Allows the quantity to be converted to the given unit.
    /// </summary>
    /// <param name="to">The unit to allow conversion to</param>
    /// <param name="quantityType">The quantity type of the unit</param>
    public QuantityMetadataBuilder<TObject> WithAllowedConversion(Enum to, Type? quantityType = null)
    {
        var conversionAttribute = new AllowUnitConversionAttribute(to, quantityType);

        AllowedConversionAttributes.AddOrUpdate(_attribute, _ => new() { conversionAttribute }, (_, bag) =>
        {
            bag.Add(conversionAttribute);
            return bag;
        });

        return this;
    }
}
