using System;

namespace UnitsNet.Metadata.Annotations;

/// <summary>
/// Allows a property to be converted to the specified unit.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class AllowUnitConversionAttribute : Attribute
{
    private readonly Lazy<Enum?> _lazyUnit;
    private readonly Lazy<Type?> _lazyQuantityType;
    private readonly Lazy<UnitInfo?> _lazyUnitInfo;
    private readonly Lazy<QuantityInfo?> _lazyQuantityInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="AllowUnitConversionAttribute"/> class.
    /// </summary>
    /// <param name="unit">The unit the property may be converted to</param>
    /// <param name="quantityType">The quantity type of the unit, or <see langword="null"/> to infer it</param>
    public AllowUnitConversionAttribute(object unit, Type? quantityType = null)
    {
        _lazyUnit = new(() =>
        {
            return unit as Enum;
        });
        _lazyUnitInfo = new(() =>
        {
            if (Unit?.TryGetUnitInfo(QuantityType, out var unitInfo) is not true)
            {
                return default;
            }

            return unitInfo!;
        });
        _lazyQuantityInfo = new(() =>
        {
            if (Unit?.TryGetQuantityInfo(QuantityType, out var quantityInfo) is not true)
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

            if (Unit?.TryGetQuantityInfo(null, out var quantityInfo) is not true)
            {
                return default;
            }

            return quantityInfo?.ValueType;
        });
    }

    /// <summary>
    /// Gets the unit the property may be converted to.
    /// </summary>
    public Enum? Unit => _lazyUnit.Value;

    /// <summary>
    /// Gets the quantity type of the unit.
    /// </summary>
    public Type? QuantityType => _lazyQuantityType.Value;
    /// <summary>
    /// Gets the UnitsNet information of the unit's quantity.
    /// </summary>
    public QuantityInfo? QuantityInfo => _lazyQuantityInfo.Value;
    /// <summary>
    /// Gets the UnitsNet information of the unit.
    /// </summary>
    public UnitInfo? UnitInfo => _lazyUnitInfo.Value;

    /// <summary>
    /// Validates that the attribute refers to a known unit.
    /// </summary>
    protected internal virtual void Validate()
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
}
