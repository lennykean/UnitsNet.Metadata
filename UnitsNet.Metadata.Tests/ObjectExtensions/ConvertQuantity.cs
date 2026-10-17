using System;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using NUnit.Framework;

using UnitsNet.Metadata.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.ObjectExtensions;

[TestFixture]
public class ConvertQuantity
{
    public readonly static TestCaseData[] ConvertQuantityTestCases = new[]
    {
        new TestCaseData(
            new ConvertQuantityTestData(1, 10, LengthUnit.Decimeter),
            new ConvertQuantityTestData(2, 200, LengthUnit.Centimeter),
            new ConvertQuantityTestData(3, 3000, LengthUnit.Millimeter),
            new ConvertQuantityTestData(4, 4000, MassUnit.Gram),
            new ConvertQuantityTestData(0, 6000, VolumeUnit.CubicDecimeter)).SetName("{c} (to same unit system)"),
        new TestCaseData(
            new ConvertQuantityTestData(1, 39.37, LengthUnit.Inch),
            new ConvertQuantityTestData(2, 6.561, LengthUnit.Foot),
            new ConvertQuantityTestData(3, 3.28, LengthUnit.Yard),
            new ConvertQuantityTestData(4, 8.818, MassUnit.Pound),
            new ConvertQuantityTestData(0, Volume.FromCubicMeters(6).CubicInches, VolumeUnit.CubicInch))
            .SetName("{c} (to different unit system)"),
        new TestCaseData(
            new ConvertQuantityTestData(1, 1, LengthUnit.Meter),
            new ConvertQuantityTestData(2, 2, LengthUnit.Meter),
            new ConvertQuantityTestData(3, 3, LengthUnit.Meter),
            new ConvertQuantityTestData(4, 4, MassUnit.Kilogram),
            new ConvertQuantityTestData(0, 6, VolumeUnit.CubicMeter)).SetName("{c} (to same unit)"),
    };

    [TestCaseSource(nameof(ConvertQuantityTestCases))]
    public void ToUnitTest(
        ConvertQuantityTestData width,
        ConvertQuantityTestData height,
        ConvertQuantityTestData depth,
        ConvertQuantityTestData weight,
        ConvertQuantityTestData volume)
    {
        var obj = new Box
        {
            Width = width.Value,
            Height = height.Value,
            Depth = depth.Value,
            Weight = weight.Value
        };

        var widthQuantity = obj.ConvertQuantity("Width", to: width.Unit);
        var heightQuantity = obj.ConvertQuantity(b => b.Height, to: height.Unit);
        var depthQuantity = obj.ConvertQuantity("Depth", to: depth.Unit);
        var volumeQuantity = obj.ConvertQuantity(b => b.Volume, to: volume.Unit);
        var weightQuantity = obj.ConvertQuantity(b => b.Weight, to: weight.Unit);

        Assert.Multiple(() =>
        {
            QuantityAssert.HasValueAndUnit(widthQuantity, width.Converted, width.Unit, 0.001);
            QuantityAssert.HasValueAndUnit(heightQuantity, height.Converted, height.Unit, 0.001);
            QuantityAssert.HasValueAndUnit(depthQuantity, depth.Converted, depth.Unit, 0.001);
            QuantityAssert.HasValueAndUnit(volumeQuantity, volume.Converted, volume.Unit, 0.001);
            QuantityAssert.HasValueAndUnit(weightQuantity, weight.Converted, weight.Unit, 0.001);
        });
    }

    [TestCase(TestName = "{c} (with interface metadata)")]
    public void WithInterfaceMetadata()
    {
        var obj = new HardDrive() as IHardDrive;

        obj.Capacity = 128;
        obj.FreeSpace = 64;

        var capacityKb = obj.ConvertQuantity("Capacity", InformationUnit.Kilobyte);
        var freespaceKb = obj.ConvertQuantity(b => b.FreeSpace, InformationUnit.Kilobyte);

        Assert.Multiple(() =>
        {
            QuantityAssert.HasValueAndUnit(capacityKb, 128_000_000, InformationUnit.Kilobyte);
            QuantityAssert.HasValueAndUnit(freespaceKb, 64_000_000, InformationUnit.Kilobyte);
        });
    }

    [TestCase(TestName = "{c} (to invalid unit)")]
    public void ToInvalidUnitTest()
    {
        var obj = new Box
        {
            Width = 1,
            Height = 2
        };

        Assert.Multiple(() =>
        {
            Assert.That(() => obj.ConvertQuantity(b => b.Width, to: AngleUnit.Degree),
                Throws.InvalidOperationException.With.Message.Match("(.*) cannot be converted to (.*)"));
            Assert.That(() => obj.ConvertQuantity("Height", to: SpeedUnit.Mach),
                Throws.InvalidOperationException.With.Message.Match("(.*) cannot be converted to (.*)"));
        });
    }

    [TestCase(TestName = "{c} (to disallowed unit)")]
    public void ToDisallowedUnitTest()
    {
        var obj = new Box
        {
            Width = 1,
            Height = 2,
            Depth = 3,
            Weight = 4
        };

        Assert.Multiple(() =>
        {
            Assert.That(() => obj.ConvertQuantity(b => b.Volume, to: VolumeUnit.CubicFoot),
                Throws.InvalidOperationException.With.Message.Match("(.*) cannot be converted to (.*)"));
            Assert.That(() => obj.ConvertQuantity("Height", to: MassUnit.Pound),
                Throws.InvalidOperationException.With.Message.Match("(.*) cannot be converted to (.*)"));
        });
    }

    [TestCase(TestName = "{c} (with invalid quantity)")]
    public void WithInvalidQuantityTest()
    {
        var obj = new Blob
        {
            Data = "1"
        };

        Assert.That(() => obj.ConvertQuantity("Data", to: InformationUnit.Gibibit),
            Throws.InvalidOperationException.With.Message.Match("(.*) is not compatible with UnitsNet.QuantityValue"));
    }

    [TestCase(TestName = "{c} (with missing property)")]
    public void WithMissingPropertyTest()
    {
        var obj = new Blob();

        Assert.That(() => obj.ConvertQuantity("FakeProperty", to: InformationUnit.Gibibit),
            Throws.InvalidOperationException.With.Message.Match("(.*) is not a property of (.*)"));
    }

    [TestCase(TestName = "{c} (with invalid attribute)")]
    public void WithInvalidAttributeTest()
    {
        var obj = new Garbage();

        Assert.That(() => obj.ConvertQuantity(r => r.Odor, to: PowerUnit.MechanicalHorsepower),
            Throws.ArgumentException.With.Message.EqualTo("Unit must be an enum value"));
    }

    [TestCase(TestName = "{c} (with custom unit)")]
    public void WithCustomUnitTest()
    {
        var obj = new Employee
        {
            Name = "Cubert",
            Coolness = 40
        };

        Assert.Multiple(() =>
        {
            QuantityAssert.HasValueAndUnit(
                obj.ConvertQuantity(e => e.Coolness, to: CoolnessUnit.Fonzie), 40_000_000, CoolnessUnit.Fonzie);
            QuantityAssert.HasValueAndUnit(
                obj.ConvertQuantity("Coolness", to: CoolnessUnit.Fonzie), 40_000_000, CoolnessUnit.Fonzie);
        });
    }

    [TestCase(TestName = "{c} (with invalid custom unit)")]
    public void WithInvalidCustomUnitTest()
    {
        var obj = new Rubbish
        {
            Coolness = 40
        };

        Assert.Multiple(() =>
        {
            Assert.That(() => obj.ConvertQuantity(r => r.Coolness, to: CoolnessUnit.Fonzie),
                Throws.ArgumentException.With.Message.Match("(.*) is not a known unit value"));
            Assert.That(() => obj.ConvertQuantity("Coolness", to: CoolnessUnit.Fonzie),
                Throws.ArgumentException.With.Message.Match("(.*) is not a known unit value"));
        });
    }

    [TestCase(TestName = "{c} (with custom attribute)")]
    public void WithCustomAttributeTest()
    {
        var obj = new DynoData
        {
            Horsepower = 300,
            Torque = 200,
            Rpm = 6000
        };

        Assert.Multiple(() =>
        {
            QuantityAssert.HasValueAndUnit(
                obj.ConvertQuantity(d => d.Horsepower, to: PowerUnit.Kilowatt), 223.7, PowerUnit.Kilowatt, 0.01);
            QuantityAssert.HasValueAndUnit(
                obj.ConvertQuantity("Torque", to: TorqueUnit.NewtonMeter), 271.16, TorqueUnit.NewtonMeter, 0.01);
        });
    }

    public record ConvertQuantityTestData(double Value, double Converted, Enum Unit);
}
