using System;
using System.Globalization;
using System.Linq;

namespace UnitsNet.Metadata.Tests.TestData;

public readonly struct Coolness : IQuantity
{
    static Coolness()
    {
        BaseDimensions = BaseDimensions.Dimensionless;
        Zero = new(0, CoolnessUnit.Fonzie);
        Info = new(
            nameof(Coolness),
            typeof(CoolnessUnit),
            new UnitInfo[]
            {
                new(CoolnessUnit.Fonzie, "fonzies", BaseUnits.Undefined),
                new(CoolnessUnit.MegaFonzie, "megafonzies", BaseUnits.Undefined)
            },
            CoolnessUnit.Fonzie,
            Zero,
            BaseDimensions.Dimensionless);

        UnitConverter.Default.SetConversionFunction<Coolness>(
            CoolnessUnit.Fonzie,
            CoolnessUnit.MegaFonzie,
            c => new(c.Value / 1_000_000, CoolnessUnit.MegaFonzie));
        UnitConverter.Default.SetConversionFunction<Coolness>(
            CoolnessUnit.MegaFonzie,
            CoolnessUnit.Fonzie,
            c => new(c.Value * 1_000_000, CoolnessUnit.Fonzie));
        UnitAbbreviationsCache.Default.MapUnitToDefaultAbbreviation(CoolnessUnit.Fonzie, "Fz");
        UnitAbbreviationsCache.Default.MapUnitToDefaultAbbreviation(CoolnessUnit.MegaFonzie, "mFz");
    }

    public Coolness(double value, CoolnessUnit unit)
    {
        Unit = unit;
        Value = value;
    }

    public static BaseDimensions BaseDimensions { get; }

    public static Coolness Zero { get; }

    public static QuantityInfo Info { get; }

    [Obsolete("QuantityType will be removed in the future..")]
    public readonly QuantityType Type => QuantityType.Undefined;

    public readonly BaseDimensions Dimensions => BaseDimensions;

    public readonly QuantityInfo QuantityInfo => Info;

    public Enum Unit { get; }

    public double Value { get; }

    public readonly double As(Enum unit)
        => UnitConverter.Default.GetConversionFunction<Coolness>(Unit, unit).Invoke(this).Value;

    public readonly string ToString(IFormatProvider? provider)
    {
        var unitAbbriviations = UnitAbbreviationsCache.Default
            .GetUnitAbbreviations(typeof(Coolness), Convert.ToInt32(Unit, CultureInfo.InvariantCulture), provider);

        return $"{Value} {unitAbbriviations.FirstOrDefault()}";
    }

    public override readonly string ToString()
    {
        return ToString(null);
    }

    public readonly IQuantity ToUnit(Enum unit)
    {
        if (unit is not CoolnessUnit coolness)
        {
            throw new ArgumentException($"Unit must be of type {typeof(CoolnessUnit)}.", nameof(unit));
        }

        return new Coolness(As(unit), coolness);
    }

    double IQuantity.As(UnitSystem unitSystem) => throw new NotImplementedException();

    string IQuantity.ToString(IFormatProvider? provider, int significantDigitsAfterRadix) => ToString(provider);

    string IQuantity.ToString(IFormatProvider? provider, string format, params object[] args) => ToString(provider);

    string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);

    IQuantity IQuantity.ToUnit(UnitSystem unitSystem) => throw new NotImplementedException();
}
