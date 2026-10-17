using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using NUnit.Framework;

using UnitsNet.Metadata.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.ObjectExtensions;

[TestFixture]
public class GetQuantity
{
    [TestCase(TestName = "{c} (with valid metadata)")]
    public void WithValidMetadataTest()
    {
        var obj = new Box
        {
            Width = 1,
            Height = 2,
            Depth = 3,
            Weight = 4,
            Items = 5
        };

        var width = obj.GetQuantity("Width");
        var height = obj.GetQuantity(b => b.Height);
        var depth = obj.GetQuantity("Depth");
        var weight = obj.GetQuantity(b => b.Weight);
        var volume = obj.GetQuantity(b => b.Volume);
        var items = obj.GetQuantity(b => b.Items);

        Assert.Multiple(() =>
        {
            QuantityAssert.HasValueAndUnit(width, 1, LengthUnit.Meter);
            QuantityAssert.HasValueAndUnit(height, 2, LengthUnit.Meter);
            QuantityAssert.HasValueAndUnit(depth, 3, LengthUnit.Meter);
            QuantityAssert.HasValueAndUnit(weight, 4, MassUnit.Kilogram);
            QuantityAssert.HasValueAndUnit(items, 5, ScalarUnit.Amount);
            QuantityAssert.HasValueAndUnit(volume, 6, VolumeUnit.CubicMeter);
        });
    }

    [TestCase(TestName = "{c} (with interface metadata)")]
    public void WithInterfaceMetadata()
    {
        var obj = new HardDrive
        {
            Manufacturer = "Seagate",
            Model = "ST1000DM003",
            Capacity = 500
        } as IHardDrive;

        obj.FreeSpace = 250;

        var capacity = obj.GetQuantity("Capacity");
        var freespace = obj.GetQuantity(h => h.FreeSpace);

        Assert.Multiple(() =>
        {
            QuantityAssert.HasValueAndUnit(capacity, 500, InformationUnit.Gigabyte);
            QuantityAssert.HasValueAndUnit(freespace, 250, InformationUnit.Gigabyte);
        });
    }

    [TestCase(TestName = "{c} (with missing metadata)")]
    public void WithMissingMetadataTest()
    {
        var obj = new Box
        {
            SerialNumber = 1,
            Priority = 2,
        };

        Assert.Multiple(() =>
        {
            Assert.That(() => obj.GetQuantity(b => b.SerialNumber),
                Throws.InvalidOperationException.With.Message.Match("Unit metadata does not exist for (.*)."));
            Assert.That(() => obj.GetQuantity("Priority"),
                Throws.InvalidOperationException.With.Message.Match("Unit metadata does not exist for (.*)."));
        });
    }

    [TestCase(TestName = "{c} (with invalid quantity)")]
    public void WithInvalidQuantityTest()
    {
        var obj = new Blob();

        Assert.That(() => obj.GetQuantity("Data"),
            Throws.InvalidOperationException.With.Message.Match("(.*) is not compatible with UnitsNet.QuantityValue"));
    }

    [TestCase(TestName = "{c} (with missing property)")]
    public void WithMissingPropertyTest()
    {
        var obj = new Blob();

        Assert.That(() => obj.GetQuantity("FakeProperty"),
            Throws.InvalidOperationException.With.Message.Match("(.*) is not a property of (.*)"));
    }

    [TestCase(TestName = "{c} (with invalid attribute)")]
    public void WithInvalidAttributeTest()
    {
        var obj = new Garbage();

        Assert.That(() => obj.GetQuantity(r => r.Odor),
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
            QuantityAssert.HasValueAndUnit(obj.GetQuantity(e => e.Coolness), 40, CoolnessUnit.MegaFonzie);
            QuantityAssert.HasValueAndUnit(obj.GetQuantity("Coolness"), 40, CoolnessUnit.MegaFonzie);
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
            Assert.That(() => obj.GetQuantity(r => r.Coolness),
                Throws.ArgumentException.With.Message.Match("(.*) is not a known unit value"));
            Assert.That(() => obj.GetQuantity("Coolness"),
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
            QuantityAssert.HasValueAndUnit(obj.GetQuantity(d => d.Horsepower), 300, PowerUnit.MechanicalHorsepower);
            QuantityAssert.HasValueAndUnit(obj.GetQuantity("Torque"), 200, TorqueUnit.PoundForceFoot);
            QuantityAssert.HasValueAndUnit(obj.GetQuantity(d => d.Rpm), 6000, RotationalSpeedUnit.RevolutionPerMinute);
        });
    }
}
