using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata;

/// <summary>
/// An attribute that can be converted into quantity metadata.
/// </summary>
/// <typeparam name="TMetadataAttribute">The attribute type</typeparam>
/// <typeparam name="TMetadata">The metadata type produced from the attribute</typeparam>
public interface IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    /// <summary>
    /// Creates the metadata described by the attribute for a property.
    /// </summary>
    /// <param name="property">The property the attribute is applied to</param>
    /// <param name="conversions">The units the quantity can be converted to</param>
    /// <param name="overrideUnit">The unit to use instead of the attribute's unit</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    TMetadata ToMetadata(
        PropertyInfo property,
        IEnumerable<UnitMetadataBasic> conversions,
        UnitMetadata? overrideUnit = null,
        CultureInfo? culture = null);
    /// <summary>
    /// Validates the attribute's configuration.
    /// </summary>
    void Validate();
}
