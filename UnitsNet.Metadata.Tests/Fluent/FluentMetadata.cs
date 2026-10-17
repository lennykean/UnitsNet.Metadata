using NUnit.Framework;

using UnitsNet.Metadata.Fluent;
using UnitsNet.Metadata.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.Fluent;

[TestFixture]
public class FluentMetadata
{
    [TestCase(TestName = "{c} (with valid metadata)")]
    public void WithValidMetadataTest()
    {
        var metadataProvider = MetadataBuilder
            .CreateFor<Cylinder>()
            .With(c => c.Radius, asUnit: LengthUnit.Meter)
            .With("Height", asUnit: LengthUnit.Meter)
            .With(c => c.Volume, asUnit: VolumeUnit.CubicMeter)
            .Build(global: false);

        var obj = new Cylinder
        {
            Radius = .1,
            Height = .5
        };

        var metadata = metadataProvider.GetObjectMetadata(obj);

        Assert.Multiple(() =>
        {
            Assert.That(metadata, Has.ItemAt(nameof(Cylinder.Radius))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Meter));
            Assert.That(metadata, Has.ItemAt(nameof(Cylinder.Height))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Meter));
            Assert.That(metadata, Has.ItemAt(nameof(Cylinder.Volume))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(VolumeUnit.CubicMeter));

            Assert.That(metadataProvider.GetQuantity(obj, c => c.Radius), Has
                .Property(nameof(IQuantity.Value)).EqualTo(0.1).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(metadataProvider.GetQuantity(obj, "Height"), Has
                .Property(nameof(IQuantity.Value)).EqualTo(.5).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(metadataProvider.GetQuantity(obj, c => c.Volume), Has
                .Property(nameof(IQuantity.Value)).EqualTo(0.016).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(VolumeUnit.CubicMeter));
        });
    }

    [TestCase(TestName = "{c} (with allowed conversions)")]
    public void WithAllowedConversionsTest()
    {
        var metadataProvider = MetadataBuilder
            .CreateFor<Cylinder>()
            .With(c => c.Radius, asUnit: LengthUnit.Meter)
                .WithAllowedConversion(to: LengthUnit.Millimeter)
                .WithAllowedConversion(to: LengthUnit.Centimeter)
            .With(c => c.Height, asUnit: LengthUnit.Meter)
                .WithAllowedConversion(to: LengthUnit.Millimeter)
                .WithAllowedConversion(to: LengthUnit.Centimeter)
            .With(c => c.Volume, asUnit: VolumeUnit.CubicMeter)
                .WithAllowedConversion(to: VolumeUnit.CubicMillimeter)
                .WithAllowedConversion(to: VolumeUnit.CubicCentimeter)
            .Build(global: false);

        var obj = new Cylinder
        {
            Radius = .1,
            Height = .5
        };

        var metadata = metadataProvider.GetObjectMetadata(obj);

        Assert.Multiple(() =>
        {
            Assert.That(metadata, Has.ItemAt(nameof(Cylinder.Radius))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Meter).And
                .ItemAt(nameof(Cylinder.Radius)).Property(nameof(QuantityMetadata.Conversions)).Count.EqualTo(3));
            Assert.That(metadata, Has.ItemAt(nameof(Cylinder.Height))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Meter).And
                .ItemAt(nameof(Cylinder.Radius)).Property(nameof(QuantityMetadata.Conversions)).Count.EqualTo(3));
            Assert.That(metadata, Has.ItemAt(nameof(Cylinder.Volume))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(VolumeUnit.CubicMeter).And
                .ItemAt(nameof(Cylinder.Radius)).Property(nameof(QuantityMetadata.Conversions)).Count.EqualTo(3));

            Assert.That(metadataProvider.GetQuantity(obj, c => c.Radius), Has
                .Property(nameof(IQuantity.Value)).EqualTo(0.1).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(metadataProvider.GetQuantity(obj, "Height"), Has
                .Property(nameof(IQuantity.Value)).EqualTo(.5).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(metadataProvider.GetQuantity(obj, c => c.Volume), Has
                .Property(nameof(IQuantity.Value)).EqualTo(0.016).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(VolumeUnit.CubicMeter));
        });
    }

    [TestCase(TestName = "{c} (global)")]
    public void GlobalTest()
    {
        MetadataBuilder
            .CreateFor<Torus>()
            .With(t => t.MinorRadius, asUnit: LengthUnit.Millimeter)
            .With(c => c.MajorRadius, asUnit: LengthUnit.Millimeter)
            .With(c => c.Volume, asUnit: VolumeUnit.CubicMillimeter)
            .Build(global: true);

        var obj = new Torus
        {
            MinorRadius = 5,
            MajorRadius = 20
        };

        var metadata = obj.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            Assert.That(metadata, Has.ItemAt(nameof(Torus.MinorRadius))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Millimeter));
            Assert.That(metadata, Has.ItemAt(nameof(Torus.MajorRadius))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Millimeter));
            Assert.That(metadata, Has.ItemAt(nameof(Torus.Volume))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(VolumeUnit.CubicMillimeter));

            Assert.That(obj.GetQuantity(t => t.MinorRadius), Has
                .Property(nameof(IQuantity.Value)).EqualTo(5).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Millimeter));
            Assert.That(obj.GetQuantity("MajorRadius"), Has
                .Property(nameof(IQuantity.Value)).EqualTo(20).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Millimeter));
            Assert.That(obj.GetQuantity(t => t.Volume), Has
                .Property(nameof(IQuantity.Value)).EqualTo(9869.60).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(VolumeUnit.CubicMillimeter));
        });
    }

    [TestCase(TestName = "{c} (with custom attribute)")]
    public void WithCustomAttributeTest()
    {
        var metadataProvider = MetadataBuilder
            .CreateFor<Chips, DisplayMeasurementAttribute, DisplayMeasurementMetadata>()
            .With(t => t.Size, new DisplayMeasurementAttribute(MassUnit.Ounce, displayName: "Net Weight"))
            .Build(global: false);

        var obj = new Chips
        {
            Name = "Salt and Vinegar",
            Size = 14.5
        };

        var metadata = metadataProvider.GetObjectMetadata(obj);

        Assert.Multiple(() =>
        {
            Assert.That(metadata, Has.ItemAt(nameof(Chips.Size))
                .Property(nameof(DisplayMeasurementMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(MassUnit.Ounce).And
                .ItemAt(nameof(Chips.Size)).Property(nameof(DisplayMeasurementMetadata
                    .DisplayName)).EqualTo("Net Weight"));

            Assert.That(metadataProvider.GetQuantity(obj, c => c.Size), Has
                .Property(nameof(IQuantity.Value)).EqualTo(14.5).Within(0.01).And
                .Property(nameof(IQuantity.Unit)).EqualTo(MassUnit.Ounce));
        });
    }
}
