using System;

namespace UnitsNet.Metadata.Tests.TestData;

public readonly struct Coolness : IQuantity<Coolness, CoolnessUnit>
{
    public Coolness(QuantityValue value, CoolnessUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    public static QuantityInfo<Coolness, CoolnessUnit> Info { get; } = new(
        CoolnessUnit.Fonzie,
        new[]
        {
            new UnitDefinition<CoolnessUnit>(CoolnessUnit.Fonzie, "fonzies", BaseUnits.Undefined),
            new UnitDefinition<CoolnessUnit>(
                CoolnessUnit.MegaFonzie,
                "megafonzies",
                BaseUnits.Undefined,
                (QuantityValue)0.000001m),
        },
        BaseDimensions.Dimensionless,
        From);

    public QuantityInfo<Coolness, CoolnessUnit> QuantityInfo => Info;

    public QuantityValue Value { get; }

    public CoolnessUnit Unit { get; }

    public UnitKey UnitKey => UnitKey.ForUnit(Unit);

    public static Coolness From(QuantityValue value, CoolnessUnit unit) => new(value, unit);

    public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);
}
