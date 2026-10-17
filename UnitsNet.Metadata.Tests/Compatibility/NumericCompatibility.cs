using System;
using System.Linq;
using System.Numerics;

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
    public void ArbitraryPrecisionAndUnsignedValuesSurviveGetSetAndProxyConversion()
    {
        var huge = BigInteger.Pow(10, 80) + 1;
        var readings = new Readings { HugeBytes = huge, UnsignedBytes = ulong.MaxValue };
        Assert.Multiple(() =>
        {
            Assert.That((BigInteger)readings.GetQuantity(r => r.HugeBytes).Value, Is.EqualTo(huge));
            Assert.That((ulong)readings.GetQuantity(r => r.UnsignedBytes).Value, Is.EqualTo(ulong.MaxValue));
        });
        readings.SetQuantity(r => r.UnsignedBytes, Information.FromBytes((QuantityValue)ulong.MaxValue));
        Assert.That(readings.UnsignedBytes, Is.EqualTo(ulong.MaxValue));

        var proxy = new[] { readings }.AsDynamicProxies()
            .WithConversion(r => r.HugeBytes, InformationUnit.Bit)
            .Build()
            .Single();
        Assert.That(proxy.HugeBytes, Is.EqualTo(huge * 8));
        proxy.HugeBytes = (huge + 1) * 8;
        Assert.That(readings.HugeBytes, Is.EqualTo(huge + 1));
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
    public void FractionalQuantityValuesAreNotRoundedThroughDecimalOrDouble()
    {
        var third = QuantityValue.One / 3;
        var readings = new Readings { FractionalBytes = third };
        var bits = readings.ConvertQuantity(r => r.FractionalBytes, InformationUnit.Bit);
        Assert.Multiple(() =>
        {
            Assert.That(readings.GetQuantity(r => r.FractionalBytes).Value, Is.EqualTo(third));
            Assert.That(bits.Value, Is.EqualTo(third * 8));
        });
        readings.SetQuantity(r => r.FractionalBytes, Information.FromBits(third * 8));
        Assert.That(readings.FractionalBytes, Is.EqualTo(third));
    }

    [Test]
    public void OverflowIsReportedInsteadOfSilentlyTruncating()
    {
        var readings = new Readings();
        Assert.Throws<OverflowException>(
            () => readings.SetQuantity(r => r.IntegerBytes, Information.FromBytes(decimal.MaxValue)));
    }

    public readonly struct CustomQuantity : IQuantity<CustomQuantity, CustomUnit>
    {
        public CustomQuantity(QuantityValue value, CustomUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        public static QuantityInfo<CustomQuantity, CustomUnit> Info { get; } = new(
            CustomUnit.Token,
            new[] { new UnitDefinition<CustomUnit>(CustomUnit.Token, "tokens", BaseUnits.Undefined) },
            BaseDimensions.Dimensionless,
            From);

        public QuantityValue Value { get; }

        public CustomUnit Unit { get; }

        public UnitKey UnitKey => UnitKey.ForUnit(Unit);

        public QuantityInfo<CustomQuantity, CustomUnit> QuantityInfo => Info;

        public static CustomQuantity From(QuantityValue value, CustomUnit unit) => new(value, unit);

        public string ToString(string? format, IFormatProvider? provider) => Value.ToString(format, provider);
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

        [Quantity(InformationUnit.Byte)]
        public virtual BigInteger HugeBytes { get; set; }

        [Quantity(InformationUnit.Byte)]
        public ulong UnsignedBytes { get; set; }

        [Quantity(InformationUnit.Byte)]
        public QuantityValue FractionalBytes { get; set; }
    }

    public class CustomReading
    {
        [Quantity(CustomUnit.Token, typeof(CustomQuantity))]
        public decimal Tokens { get; set; }
    }
}
