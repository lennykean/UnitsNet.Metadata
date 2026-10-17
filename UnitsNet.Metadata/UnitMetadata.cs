using System;
using System.Collections.Concurrent;
using System.Globalization;

using UnitsNet.Metadata.Reflection;
using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata;

/// <summary>
/// The metadata of a unit that can convert values to other units.
/// </summary>
public sealed class UnitMetadata : UnitMetadataBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitMetadata"/> class.
    /// </summary>
    /// <param name="unitInfo">The UnitsNet unit information</param>
    /// <param name="name">The unit name</param>
    /// <param name="value">The numeric value of the unit enumeration</param>
    /// <param name="displayName">The human-readable name of the unit</param>
    /// <param name="abbriviation">The unit abbreviation</param>
    /// <param name="quantityType">The metadata of the quantity the unit belongs to</param>
    public UnitMetadata(
        UnitInfo unitInfo,
        string name,
        int value,
        string displayName,
        string abbriviation,
        QuantityTypeMetadata quantityType)
        : base(unitInfo, name, value, displayName, abbriviation)
    {
        QuantityType = quantityType;
    }

    /// <summary>
    /// Gets the metadata of the quantity the unit belongs to.
    /// </summary>
    public QuantityTypeMetadata QuantityType { get; }

    /// <summary>
    /// Gets the metadata of a unit.
    /// </summary>
    /// <param name="unitInfo">The unit to describe</param>
    /// <param name="quantityInfo">The quantity the unit belongs to</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public static UnitMetadata FromUnitInfo(UnitInfo unitInfo, QuantityInfo quantityInfo, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.CurrentCulture;
        var cache = EphemeralValueCache<(Enum, CultureInfo), UnitMetadata>.GlobalInstance;
        return cache.GetOrAdd((unitInfo.Value, culture), _ =>
        {
            var quantityType = QuantityTypeMetadata.FromQuantityInfo(quantityInfo, culture);
            return new(
                unitInfo,
                unitInfo.Name,
                Convert.ToInt32(unitInfo.Value),
                GetDisplayName(unitInfo),
                GetAbbriviation(unitInfo, quantityInfo, culture),
                quantityType);
        });
    }

    /// <summary>
    /// Tries to convert a value in this unit to another unit.
    /// </summary>
    /// <param name="value">The value in this unit</param>
    /// <param name="to">The unit to convert to</param>
    /// <param name="convertedQuantity">The converted quantity</param>
    public bool TryConvertQuantity(double value, UnitMetadata to, out IQuantity convertedQuantity)
        => TryConvertQuantity((QuantityValue)value, to, out convertedQuantity);

    /// <summary>
    /// Tries to convert a value in this unit to another unit.
    /// </summary>
    /// <param name="value">The value in this unit</param>
    /// <param name="to">The unit to convert to</param>
    /// <param name="convertedQuantity">The converted quantity</param>
    public bool TryConvertQuantity(QuantityValue value, UnitMetadata to, out IQuantity convertedQuantity)
    {
        convertedQuantity = value.AsQuantity(UnitInfo.Value, QuantityType.QuantityInfo.ValueType);
        if (UnitInfo.Value.Equals(to.UnitInfo.Value))
        {
            return true;
        }

        var conversionFunctions =
            EphemeralKeyCache<UnitMetadata, ConcurrentDictionary<UnitMetadata, ConversionFunction>>.GlobalInstance
                .GetOrAdd(this, _ => new());
        if (!conversionFunctions.TryGetValue(to, out var conversionFunction))
        {
            if (!UnitConverter.Default.TryGetConversionFunction(
                    QuantityType.QuantityInfo.ValueType, UnitInfo.Value,
                    to.QuantityType.QuantityInfo.ValueType,
                    to.UnitInfo.Value,
                    out conversionFunction))
            {
                var baseUnit = QuantityType.BaseUnit;
                if (baseUnit is null || baseUnit != to.QuantityType.BaseUnit)
                {
                    return false;
                }

                if (!UnitConverter.Default.TryGetConversionFunction(
                        QuantityType.QuantityInfo.ValueType,
                        UnitInfo.Value,
                        baseUnit.QuantityType.QuantityInfo.ValueType,
                        baseUnit.UnitInfo.Value,
                        out var baseUnitConversionFunction))
                {
                    return false;
                }

                if (!UnitConverter.Default.TryGetConversionFunction(
                        baseUnit.QuantityType.QuantityInfo.ValueType,
                        baseUnit.UnitInfo.Value,
                        to.QuantityType.QuantityInfo.ValueType,
                        to.UnitInfo.Value,
                        out var finalUnitConversionFunction))
                {
                    return false;
                }

                conversionFunction = quantity => finalUnitConversionFunction(baseUnitConversionFunction(quantity));
            }

            conversionFunctions.AddOrUpdate(to, conversionFunction, (_, _) => conversionFunction);
        }

        convertedQuantity = conversionFunction.Invoke(convertedQuantity);
        return true;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is UnitMetadata other && UnitInfo.Value.Equals(other.UnitInfo.Value);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(UnitInfo.Value);
    }
}
