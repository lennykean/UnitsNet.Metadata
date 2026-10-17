using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace UnitsNet.Metadata.Annotations;

/// <summary>
/// Marks a property with the unit its value is expressed in.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class QuantityAttribute : Attribute, IMetadataAttribute<QuantityAttribute, QuantityMetadata>
{
    private readonly Lazy<Enum?> _lazyUnit;
    private readonly Lazy<Type?> _lazyQuantityType;
    private readonly Lazy<UnitInfo?> _lazyUnitInfo;
    private readonly Lazy<QuantityInfo?> _lazyQuantityInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuantityAttribute"/> class.
    /// </summary>
    /// <param name="unit">The unit enum value of the property</param>
    /// <param name="quantityType">The quantity type, or <see langword="null"/> to infer it from the unit</param>
    public QuantityAttribute(object unit, Type? quantityType = null)
    {
        _lazyUnit = new(() =>
        {
            return unit as Enum;
        });
        _lazyUnitInfo = new(() =>
        {
            if (Unit?.TryGetUnitInfo(quantityType, out var unitInfo) is not true)
            {
                return default;
            }

            return unitInfo;
        });
        _lazyQuantityInfo = new(() =>
        {
            if (Unit?.TryGetQuantityInfo(quantityType, out var quantityInfo) is not true)
            {
                return default;
            }

            return quantityInfo!;
        });
        _lazyQuantityType = new(() =>
        {
            if (quantityType is not null)
            {
                return quantityType;
            }

            if (Unit?.TryGetQuantityInfo(quantityType, out var quantityInfo) is not true)
            {
                return default;
            }

            return quantityInfo?.QuantityType;
        });
    }

    /// <summary>
    /// Gets the unit of the property.
    /// </summary>
    public Enum? Unit => _lazyUnit.Value;

    /// <summary>
    /// Gets the quantity type of the property.
    /// </summary>
    public Type? QuantityType => _lazyQuantityType?.Value;
    /// <summary>
    /// Gets the UnitsNet information of the property's quantity.
    /// </summary>
    public QuantityInfo? QuantityInfo => _lazyQuantityInfo?.Value;
    /// <summary>
    /// Gets the UnitsNet information of the property's unit.
    /// </summary>
    public UnitInfo? UnitInfo => _lazyUnitInfo?.Value;

    /// <summary>
    /// Builds the metadata of the units the property can be converted to.
    /// </summary>
    /// <param name="allowedConversions">The conversions allowed on the property</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public IEnumerable<UnitMetadataBasic> BuildAllowedConversionsMetadata(
        IEnumerable<AllowUnitConversionAttribute> allowedConversions,
        CultureInfo? culture = null)
    {
        if (UnitInfo is null || QuantityInfo is null)
        {
            yield break;
        }

        var selfConversion = UnitMetadataBasic.FromUnitInfo(UnitInfo, QuantityInfo, culture)!;

        var distinctConversions = new HashSet<UnitMetadataBasic>()
        {
            selfConversion
        };
        yield return selfConversion;

        if (allowedConversions.Any())
        {
            foreach (var conversion in allowedConversions)
            {
                conversion.Validate();

                var (unitInfo, quantityInfo) = GetConversionInfo(conversion);
                var unitMetadata = UnitMetadataBasic.FromUnitInfo(unitInfo, quantityInfo, culture);
                if (unitMetadata is not null && !distinctConversions.Contains(unitMetadata))
                {
                    distinctConversions.Add(unitMetadata);
                    yield return unitMetadata;
                }
            }
        }
        else
        {
            foreach (var unit in QuantityInfo.UnitInfos)
            {
                var unitMetadata = UnitMetadataBasic.FromUnitInfo(unit, QuantityInfo, culture);
                if (unitMetadata is not null && !distinctConversions.Contains(unitMetadata))
                {
                    distinctConversions.Add(unitMetadata);
                    yield return unitMetadata;
                }
            }
        }
    }

    /// <summary>
    /// Validates that the attribute holds a known unit.
    /// </summary>
    public virtual void Validate()
    {
        if (Unit is null)
        {
            throw new ArgumentException($"{nameof(Unit)} must be an enum value");
        }

        if (!Unit.TryGetUnitInfo(QuantityType, out _))
        {
            throw new ArgumentException($"{Unit?.GetType()}.{Unit} is not a known unit value.");
        }

        if (!Unit.TryGetQuantityInfo(QuantityType, out _))
        {
            throw new ArgumentException($"{Unit?.GetType()} is not a known unit type.");
        }
    }

    QuantityMetadata IMetadataAttribute<QuantityAttribute, QuantityMetadata>.ToMetadata(
        PropertyInfo property,
        IEnumerable<UnitMetadataBasic> conversions,
        UnitMetadata? overrideUnit,
        CultureInfo? culture)
    {
        var unit = overrideUnit;
        if (unit is null && UnitInfo is not null && QuantityInfo is not null)
        {
            unit = UnitMetadata.FromUnitInfo(UnitInfo, QuantityInfo, culture);
        }

        return new(property, unit, conversions.ToList());
    }

    private (UnitInfo unitInfo, QuantityInfo quantityInfo) GetConversionInfo(
        AllowUnitConversionAttribute allowedConversion)
    {
        var unitInfo = allowedConversion.UnitInfo;
        var quantityInfo = allowedConversion.QuantityInfo;

        if (unitInfo is not null && quantityInfo is not null)
        {
            return (unitInfo, quantityInfo);
        }

        if (quantityInfo is null && QuantityInfo?.UnitInfos.Any(u => u.Value.Equals(Unit)) == true)
        {
            quantityInfo = QuantityInfo;
        }

        if (quantityInfo is null ||
            unitInfo is null && Unit?.TryGetUnitInfo(quantityInfo.QuantityType, out unitInfo) is not true)
        {
            return default;
        }

        return (unitInfo!, quantityInfo);
    }
}
