using System.Collections.Generic;

using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using NUnit.Framework;

using UnitsNet.Metadata.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.ObjectExtensions;

[TestFixture]
public class GetObjectMetadata
{
    [TestCase(TestName = "{c} (with valid metadata)")]
    public void WithValidMetadataTest()
    {
        var box = new Box();

        var metadata = box.GetObjectMetadata();
        var collectionMetadata = new List<Box> { box }.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            Assert.That(collectionMetadata, Is.EquivalentTo(metadata));
            Assert.That(metadata, Has.Count.EqualTo(6));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Width))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Meter));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Height))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Meter));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Depth))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Meter));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Weight))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(MassUnit.Kilogram));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Items))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(ScalarUnit.Amount));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Volume))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(VolumeUnit.CubicMeter));
        });
    }

    [TestCase(TestName = "{c} (with interface metadata)")]
    public void WithInterfaceMetadata()
    {
        var obj = new HardDrive() as IHardDrive;

        var metadata = obj.GetObjectMetadata();
        var collectionMetadata = new List<IHardDrive> { obj }.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            Assert.That(collectionMetadata, Is.EquivalentTo(metadata));
            Assert.That(metadata, Has.Count.EqualTo(2));
            Assert.That(metadata, Has.ItemAt(nameof(IHardDrive.Capacity))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(InformationUnit.Gigabyte));
            Assert.That(metadata, Has.ItemAt(nameof(IHardDrive.FreeSpace))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(InformationUnit.Gigabyte));
        });
    }

    [TestCase(TestName = "{c} (with invalid quantity)")]
    public void WithInvalidQuantityTest()
    {
        var blob = new Blob
        {
            Data = "1"
        };

        Assert.That(() => blob.GetObjectMetadata(),
            Throws.InvalidOperationException.With.Message.Match("(.*) is not compatible with UnitsNet.QuantityValue"));
    }

    [TestCase(TestName = "{c} (with invalid attribute)")]
    public void WithInvalidAttributeTest()
    {
        var garbage = new Garbage();

        Assert.That(() => garbage.GetObjectMetadata(),
            Throws.ArgumentException.With.Message.EqualTo("Unit must be an enum value"));
    }

    [TestCase(TestName = "{c} (with custom unit)")]
    public void WithCustomUnitTest()
    {
        var employee = new Employee();

        var metadata = employee.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            Assert.That(metadata, Has.Count.EqualTo(1));
            Assert.That(metadata, Has.ItemAt(nameof(employee.Coolness))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(CoolnessUnit.MegaFonzie));
        });
    }

    [TestCase(TestName = "{c} (with invalid custom unit)")]
    public void WithInvlidCustomUnitTest()
    {
        var rubbish = new Rubbish
        {
            Coolness = 40
        };

        Assert.That(() => rubbish.GetObjectMetadata(),
            Throws.ArgumentException.With.Message.Match("(.*) is not a known unit value"));
    }

    [TestCase(TestName = "{c} (with custom attribute)")]
    public void WithCustomAttributeTest()
    {
        var obj = new DynoData();

        var metadata = obj.GetObjectMetadata();
        var collectionMetadata = new[] { obj }.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            Assert.That(collectionMetadata, Is.EquivalentTo(metadata));
            Assert.That(metadata, Has.Count.EqualTo(3));
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Horsepower))
                .Property(nameof(DisplayMeasurementMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(PowerUnit.MechanicalHorsepower));
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Torque))
                .Property(nameof(DisplayMeasurementMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(TorqueUnit.PoundForceFoot));
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Rpm))
                .Property(nameof(DisplayMeasurementMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(RotationalSpeedUnit.RevolutionPerMinute));
        });
    }

    [TestCase(TestName = "{c} (typed with custom attribute)")]
    public void TypedWithCustomAttributeTest()
    {
        var obj = new DynoData();

        var metadata = obj.GetObjectMetadata<DynoData, DisplayMeasurementAttribute, DisplayMeasurementMetadata>();
        var collectionMetadata = new List<DynoData> { obj }
            .GetObjectMetadata<IEnumerable<DynoData>, DisplayMeasurementAttribute, DisplayMeasurementMetadata>();

        Assert.Multiple(() =>
        {
            Assert.That(collectionMetadata, Is.EquivalentTo(metadata));
            Assert.That(metadata, Has.Count.EqualTo(3));
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Horsepower))
                .Property(nameof(DisplayMeasurementMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(PowerUnit.MechanicalHorsepower).And
                .ItemAt(nameof(DynoData.Horsepower)).Property(nameof(DisplayMeasurementMetadata
                    .DisplayName)).EqualTo("Engine Horsepower"));
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Torque))
                .Property(nameof(DisplayMeasurementMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(TorqueUnit.PoundForceFoot).And
                .ItemAt(nameof(DynoData.Torque)).Property(nameof(DisplayMeasurementMetadata
                    .DisplayName)).EqualTo("Engine Torque"));
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Rpm))
                .Property(nameof(DisplayMeasurementMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(RotationalSpeedUnit.RevolutionPerMinute).And
                .ItemAt(nameof(DynoData.Rpm)).Property(nameof(DisplayMeasurementMetadata
                    .DisplayName)).EqualTo("Engine Speed"));
        });
    }
}
