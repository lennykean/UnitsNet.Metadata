using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using NUnit.Framework;

using UnitsNet.Metadata.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.ObjectExtensions;

[TestFixture]
public class SetQuantity
{
    [TestCase(TestName = "{c} (with valid metadata)")]
    public void WithValidMetadataTest()
    {
        var obj = new Box();

        var width = obj.SetQuantity("Width", Length.FromCentimeters(100));
        var height = obj.SetQuantity(b => b.Height, Length.FromCentimeters(200));
        var depth = obj.SetQuantity("Depth", Length.FromCentimeters(300));
        var weight = obj.SetQuantity(b => b.Weight, Mass.FromGrams(4000));
        var items = obj.SetQuantity(b => b.Items, Scalar.FromAmount(5));

        Assert.Multiple(() =>
        {
            Assert.That(obj, Has.Property(nameof(Box.Width)).EqualTo(1));
            Assert.That(width, Has
                .Property(nameof(IQuantity.Value)).EqualTo(1).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(obj, Has.Property(nameof(Box.Height)).EqualTo(2));
            Assert.That(height, Has
                .Property(nameof(IQuantity.Value)).EqualTo(2).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(obj, Has.Property(nameof(Box.Depth)).EqualTo(3));
            Assert.That(depth, Has
                .Property(nameof(IQuantity.Value)).EqualTo(3).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(obj, Has.Property(nameof(Box.Weight)).EqualTo(4));
            Assert.That(weight, Has
                .Property(nameof(IQuantity.Value)).EqualTo(4).And
                .Property(nameof(IQuantity.Unit)).EqualTo(MassUnit.Kilogram));
            Assert.That(obj, Has.Property(nameof(Box.Items)).EqualTo(5));
            Assert.That(items, Has
                .Property(nameof(IQuantity.Value)).EqualTo(5).And
                .Property(nameof(IQuantity.Unit)).EqualTo(ScalarUnit.Amount));
        });
    }

    [TestCase(TestName = "{c} (with interface metadata)")]
    public void WithInterfaceMetadata()
    {
        var obj = new HardDrive() as IHardDrive;

        var capacity = obj.SetQuantity("Capacity", Information.FromTerabytes(1));
        var freespace = obj.SetQuantity(b => b.FreeSpace, Information.FromTerabytes(0.5));

        Assert.Multiple(() =>
        {
            Assert.That(capacity, Has
                .Property(nameof(IQuantity.Value)).EqualTo(1000).And
                .Property(nameof(IQuantity.Unit)).EqualTo(InformationUnit.Gigabyte));
            Assert.That(freespace, Has
                .Property(nameof(IQuantity.Value)).EqualTo(500).And
                .Property(nameof(IQuantity.Unit)).EqualTo(InformationUnit.Gigabyte));
        });
    }

    [TestCase(TestName = "{c} (with missing metadata)")]
    public void WithMissingMetadataTest()
    {
        var obj = new Box();

        Assert.Multiple(() =>
        {
            Assert.That(() => obj.SetQuantity(b => b.SerialNumber, Length.FromCentimeters(10)),
                Throws.InvalidOperationException.With.Message.Match("Unit metadata does not exist for (.*)."));
            Assert.That(() => obj.SetQuantity("Priority", Length.FromCentimeters(10)),
                Throws.InvalidOperationException.With.Message.Match("Unit metadata does not exist for (.*)."));
        });
    }

    [TestCase(TestName = "{c} (with invalid quantity)")]
    public void WithInvalidQuantityTest()
    {
        var obj = new Blob();

        Assert.That(() => obj.SetQuantity("Data", Information.FromBytes(42)),
            Throws.InvalidOperationException.With.Message.Match("(.*) is not compatible with UnitsNet.QuantityValue"));
    }

    [TestCase(TestName = "{c} (with missing property)")]
    public void WithMissingPropertyTest()
    {
        var obj = new Blob();

        Assert.That(() => obj.SetQuantity("FakeProperty", Length.FromCentimeters(10)),
            Throws.InvalidOperationException.With.Message.Match("(.*) is not a property of (.*)"));
    }

    [TestCase(TestName = "{c} (with invalid attribute)")]
    public void WithInvalidAttributeTest()
    {
        var obj = new Garbage();

        Assert.That(() => obj.SetQuantity(r => r.Odor, Scalar.FromAmount(200)),
            Throws.ArgumentException.With.Message.EqualTo("Unit must be an enum value"));
    }

    [TestCase(TestName = "{c} (with custom unit)")]
    public void WithCustomUnitTest()
    {
        var obj = new Employee();

        var coolness = obj.SetQuantity(e => e.Coolness, new Coolness(40000000, CoolnessUnit.Fonzie));

        Assert.Multiple(() =>
        {
            Assert.That(obj, Has.Property(nameof(Employee.Coolness)).EqualTo(40));
            Assert.That(coolness, Has
                .Property(nameof(IQuantity.Value)).EqualTo(40).And
                .Property(nameof(IQuantity.Unit)).EqualTo(CoolnessUnit.MegaFonzie));
        });
    }

    [TestCase(TestName = "{c} (with invalid custom unit)")]
    public void WithInvalidCustomUnitTest()
    {
        var obj = new Rubbish();

        Assert.Multiple(() =>
        {
            Assert.That(() => obj.SetQuantity(r => r.Coolness, new Coolness(40000000, CoolnessUnit.Fonzie)),
                Throws.ArgumentException.With.Message.Match("(.*) is not a known unit type"));
            Assert.That(() => obj.SetQuantity("Coolness", new Coolness(40000000, CoolnessUnit.Fonzie)),
                Throws.ArgumentException.With.Message.Match("(.*) is not a known unit type"));
        });
    }

    [TestCase(TestName = "{c} (with custom attribute)")]
    public void WithCustomAttributeTest()
    {
        var obj = new DynoData();

        var horsepower = obj.SetQuantity(d => d.Horsepower, Power.FromKilowatts(400));
        var torque = obj.SetQuantity(d => d.Torque, Torque.FromNewtonMeters(300));
        var rpm = obj.SetQuantity(d => d.Rpm, RotationalSpeed.FromRadiansPerSecond(500));

        Assert.Multiple(() =>
        {
            Assert.That(obj, Has.Property(nameof(DynoData.Horsepower)).EqualTo(536.41).Within(0.01));
            Assert.That(horsepower, Has
                .Property(nameof(IQuantity.Value)).EqualTo(536.41).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(PowerUnit.MechanicalHorsepower));
            Assert.That(obj, Has.Property(nameof(DynoData.Torque)).EqualTo(221.27).Within(0.01));
            Assert.That(torque, Has
                .Property(nameof(IQuantity.Value)).EqualTo(221.27).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(TorqueUnit.PoundForceFoot));
            Assert.That(obj, Has.Property(nameof(DynoData.Rpm)).EqualTo(4774.65).Within(0.01));
            Assert.That(rpm, Has
                .Property(nameof(IQuantity.Value)).EqualTo(4774.65).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(RotationalSpeedUnit.RevolutionPerMinute));
        });
    }
}
