using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using UnitsNet.Metadata.Reflection;

namespace UnitsNet.Metadata;

/// <summary>
/// The unit and allowed conversions of a quantity property.
/// </summary>
public class QuantityMetadata : IMetadata<QuantityMetadata>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuantityMetadata"/> class.
    /// </summary>
    /// <param name="property">The quantity property</param>
    /// <param name="unit">The unit of the property, or <see langword="null"/> when it has none</param>
    /// <param name="conversions">The units the property can be converted to</param>
    public QuantityMetadata(PropertyInfo property, UnitMetadata? unit, IList<UnitMetadataBasic> conversions)
    {
        Property = property;
        Unit = unit;
        Conversions = new(conversions);
    }

    /// <summary>
    /// Gets the quantity property.
    /// </summary>
    [JsonIgnore, IgnoreDataMember]
    public PropertyInfo Property { get; }

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    public string FieldName => Property.Name;
    /// <summary>
    /// Gets the unit of the property, or <see langword="null"/> when it has none.
    /// </summary>
    public UnitMetadata? Unit { get; }
    /// <summary>
    /// Gets the units the property can be converted to.
    /// </summary>
    public ReadOnlyCollection<UnitMetadataBasic> Conversions { get; }

    /// <summary>
    /// Validates that the property is a supported quantity type.
    /// </summary>
    public virtual void Validate()
    {
        Property.PropertyType.ValidateQuantityType();
    }

    QuantityMetadata IMetadata<QuantityMetadata>.Clone(
        PropertyInfo? overrideProperty,
        IEnumerable<UnitMetadataBasic>? overrideConversions,
        UnitMetadata? overrideUnit,
        CultureInfo? overrideCulture)
    {
        return new QuantityMetadata(
            overrideProperty ?? Property,
            overrideUnit ?? Unit,
            (overrideConversions ?? Conversions).ToList());
    }
}
