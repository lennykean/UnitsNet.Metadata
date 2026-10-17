using System;
using System.Linq;

using NUnit.Framework;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.DynamicProxy;
using UnitsNet.Metadata.Fluent;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.Compatibility;

[TestFixture]
public class NumericCompatibility
{
    private const decimal PreciseValue = 9007199254740993.125m;

    public enum CustomUnit
    {
        Token,
    }

    [Test]
    public void CustomQuantityValueConstructorPreservesDecimals()
    {
        var reading = new CustomReading { Tokens = PreciseValue };
        Assert.That((decimal)reading.GetQuantity(r => r.Tokens).Value, Is.EqualTo(PreciseValue));
        reading.SetQuantity(r => r.Tokens, new CustomQuantity(PreciseValue + 1m, CustomUnit.Token));
        Assert.That(reading.Tokens, Is.EqualTo(PreciseValue + 1m));
    }

    [Test]
    public void DecimalAndLongValuesSurviveGetSetAndConversion()
    {
        var readings = new Readings { Bytes = PreciseValue, IntegerBytes = 9007199254740993L };
        var kilobytes = readings.ConvertQuantity(r => r.Bytes, InformationUnit.Kilobyte);
        Assert.Multiple(() =>
        {
            Assert.That((decimal)readings.GetQuantity(r => r.Bytes).Value, Is.EqualTo(PreciseValue));
            Assert.That((decimal)readings.GetQuantity(r => r.IntegerBytes).Value, Is.EqualTo(9007199254740993m));
            Assert.That((decimal)kilobytes.Value, Is.EqualTo(PreciseValue / 1000m));
        });

        readings.SetQuantity(r => r.Bytes, Information.FromKilobytes(PreciseValue / 1000m));
        readings.SetQuantity(r => r.IntegerBytes, Information.FromBytes(9007199254740993m));
        Assert.Multiple(() =>
        {
            Assert.That(readings.Bytes, Is.EqualTo(PreciseValue));
            Assert.That(readings.IntegerBytes, Is.EqualTo(9007199254740993L));
        });
    }

    [Test]
    public void DecimalProxyRoundTripsWithoutDoubleRounding()
    {
        var readings = new Readings { Bytes = PreciseValue };
        var proxy = new[] { readings }.AsDynamicProxies()
            .WithConversion(r => r.Bytes, InformationUnit.Kilobyte)
            .Build()
            .Single();
        Assert.That(proxy.Bytes, Is.EqualTo(PreciseValue / 1000m));
        proxy.Bytes = (PreciseValue + 1m) / 1000m;
        Assert.That(readings.Bytes, Is.EqualTo(PreciseValue + 1m));
    }

    [Test]
    public void DoubleFloatAndOffsetConversionsStillWork()
    {
        var readings = new Readings();
        readings.SetQuantity(r => r.Length, Length.FromCentimeters(125));
        readings.SetQuantity(r => r.Temperature, Temperature.FromDegreesFahrenheit(212));
        Assert.That(readings.Length, Is.EqualTo(1.25));
        Assert.That(readings.Temperature, Is.EqualTo(100f).Within(0.0001));
    }

    [Test]
    public void FluentMetadataPreservesDecimalValues()
    {
        var provider = MetadataBuilder.CreateFor<Readings>()
            .With(r => r.Bytes, InformationUnit.Byte)
            .Build(global: false);
        var readings = new Readings { Bytes = PreciseValue };
        Assert.That((decimal)provider.GetQuantity(readings, r => r.Bytes).Value, Is.EqualTo(PreciseValue));
        provider.SetQuantity(readings, r => r.Bytes, Information.FromBytes(PreciseValue + 1m));
        Assert.That(readings.Bytes, Is.EqualTo(PreciseValue + 1m));
    }

    [Test]
    public void OverflowIsReportedInsteadOfSilentlyTruncating()
    {
        var readings = new Readings();
        Assert.Throws<OverflowException>(
            () => readings.SetQuantity(r => r.IntegerBytes, Information.FromBytes(decimal.MaxValue)));
    }

    public readonly struct CustomQuantity : IQuantity
    {
        static CustomQuantity()
        {
            UnitAbbreviationsCache.Default.MapUnitToDefaultAbbreviation(CustomUnit.Token, "tk");
        }

        public CustomQuantity(QuantityValue value, CustomUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        public static QuantityInfo Info { get; } = new(
            nameof(CustomQuantity),
            typeof(CustomUnit),
            new[] { new UnitInfo(CustomUnit.Token, "tokens", BaseUnits.Undefined) },
            CustomUnit.Token,
            new CustomQuantity(0, CustomUnit.Token),
            BaseDimensions.Dimensionless);

        public QuantityValue Value { get; }

        public Enum Unit { get; }

        public QuantityInfo QuantityInfo => Info;

        public BaseDimensions Dimensions => BaseDimensions.Dimensionless;

        public double As(Enum unit) => unit.Equals(Unit) ? (double)Value : throw new ArgumentException("Unknown unit.");

        public double As(UnitSystem unitSystem) => (double)Value;

        public IQuantity ToUnit(Enum unit) => unit.Equals(Unit) ? this : throw new ArgumentException("Unknown unit.");

        public IQuantity ToUnit(UnitSystem unitSystem) => this;

        public string ToString(IFormatProvider? provider) => Value.ToString(provider!);

        public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);

        public bool Equals(IQuantity? other, IQuantity tolerance)
            => other is CustomQuantity quantity &&
                Math.Abs((decimal)Value - (decimal)quantity.Value) <= (decimal)tolerance.Value;
    }

    public class Readings
    {
        [Quantity(InformationUnit.Byte)]
        public virtual decimal Bytes { get; set; }

        [Quantity(InformationUnit.Byte)]
        public long IntegerBytes { get; set; }

        [Quantity(LengthUnit.Meter)]
        public double Length { get; set; }

        [Quantity(TemperatureUnit.DegreeCelsius)]
        public float Temperature { get; set; }
    }

    public class CustomReading
    {
        [Quantity(CustomUnit.Token, typeof(CustomQuantity))]
        public decimal Tokens { get; set; }
    }
}
