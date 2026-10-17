using System;
using System.Globalization;

using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata;

/// <summary>
/// The metadata of a quantity type, including its base unit.
/// </summary>
public sealed class QuantityTypeMetadata : QuantityTypeMetadataBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuantityTypeMetadata"/> class.
    /// </summary>
    /// <param name="quantityInfo">The UnitsNet information of the quantity type</param>
    /// <param name="name">The name of the quantity type</param>
    /// <param name="displayName">The human-readable name of the quantity type</param>
    /// <param name="baseUnit">The base unit of the quantity type</param>
    public QuantityTypeMetadata(
        QuantityInfo quantityInfo,
        string name,
        string displayName,
        UnitMetadataBasic? baseUnit) : base(quantityInfo, name, displayName)
    {
        BaseUnit = baseUnit;
    }

    /// <summary>
    /// Gets the base unit of the quantity type.
    /// </summary>
    public UnitMetadataBasic? BaseUnit { get; }

    /// <summary>
    /// Gets the metadata of a quantity type.
    /// </summary>
    /// <param name="quantityInfo">The UnitsNet information of the quantity type</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public static QuantityTypeMetadata FromQuantityInfo(QuantityInfo quantityInfo, CultureInfo? culture = null)
    {
        return EphemeralValueCache<Type, QuantityTypeMetadata>.GlobalInstance.GetOrAdd(quantityInfo.ValueType, _ =>
        {
            var name = quantityInfo.Name;
            var displayName = GetDisplayName(quantityInfo);
            var baseUnit = UnitMetadataBasic.FromUnitInfo(quantityInfo.BaseUnitInfo, quantityInfo, culture);

            return new(quantityInfo, name, displayName, baseUnit);
        });
    }
}
