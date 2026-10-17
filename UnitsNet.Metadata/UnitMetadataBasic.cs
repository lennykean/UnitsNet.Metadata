using System;
using System.Globalization;

using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata;

/// <summary>
/// The basic metadata of a unit.
/// </summary>
public sealed class UnitMetadataBasic : UnitMetadataBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitMetadataBasic"/> class.
    /// </summary>
    /// <param name="unitInfo">The UnitsNet unit information</param>
    /// <param name="name">The unit name</param>
    /// <param name="value">The numeric value of the unit enumeration</param>
    /// <param name="displayName">The human-readable name of the unit</param>
    /// <param name="abbriviation">The unit abbreviation</param>
    /// <param name="quantityType">The metadata of the quantity the unit belongs to</param>
    public UnitMetadataBasic(
        UnitInfo unitInfo,
        string name,
        int value,
        string displayName,
        string abbriviation,
        QuantityTypeMetadataBasic quantityType)
        : base(unitInfo, name, value, displayName, abbriviation)
    {
        QuantityType = quantityType;
    }

    /// <summary>
    /// Gets the metadata of the quantity the unit belongs to.
    /// </summary>
    public QuantityTypeMetadataBasic QuantityType { get; }

    /// <summary>
    /// Gets the metadata of a unit, or <see langword="null"/> when its quantity has none.
    /// </summary>
    /// <param name="unitInfo">The unit to describe</param>
    /// <param name="quantityInfo">The quantity the unit belongs to</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public static UnitMetadataBasic? FromUnitInfo(
        UnitInfo unitInfo,
        QuantityInfo quantityInfo,
        CultureInfo? culture = null)
    {
        return EphemeralValueCache<Enum, UnitMetadataBasic?>.GlobalInstance.GetOrAdd(unitInfo.Value, _ =>
        {
            return QuantityTypeMetadataBasic.FromQuantityInfo(quantityInfo) switch
            {
                var quantityType when quantityType is not null
                    => new(
                        unitInfo,
                        unitInfo.Name,
                        Convert.ToInt32(unitInfo.Value),
                        GetDisplayName(unitInfo),
                        GetAbbriviation(unitInfo, quantityInfo, culture),
                        quantityType),
                _
                    => null
            };
        });
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is UnitMetadataBasic other && UnitInfo.Value.Equals(other.UnitInfo.Value);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(UnitInfo.Value);
    }
}
