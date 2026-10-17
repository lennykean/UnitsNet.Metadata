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

            QuantityAssert.HasValueAndUnit(
                metadataProvider.GetQuantity(obj, c => c.Radius), 0.1, LengthUnit.Meter, 0.01);
            QuantityAssert.HasValueAndUnit(metadataProvider.GetQuantity(obj, "Height"), .5, LengthUnit.Meter, 0.01);
            QuantityAssert.HasValueAndUnit(
                metadataProvider.GetQuantity(obj, c => c.Volume), 0.016, VolumeUnit.CubicMeter, 0.01);
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

            QuantityAssert.HasValueAndUnit(
                metadataProvider.GetQuantity(obj, c => c.Radius), 0.1, LengthUnit.Meter, 0.01);
            QuantityAssert.HasValueAndUnit(metadataProvider.GetQuantity(obj, "Height"), .5, LengthUnit.Meter, 0.01);
            QuantityAssert.HasValueAndUnit(
                metadataProvider.GetQuantity(obj, c => c.Volume), 0.016, VolumeUnit.CubicMeter, 0.01);
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

            QuantityAssert.HasValueAndUnit(obj.GetQuantity(t => t.MinorRadius), 5, LengthUnit.Millimeter, 0.01);
            QuantityAssert.HasValueAndUnit(obj.GetQuantity("MajorRadius"), 20, LengthUnit.Millimeter, 0.01);
            QuantityAssert.HasValueAndUnit(obj.GetQuantity(t => t.Volume), 9869.60, VolumeUnit.CubicMillimeter, 0.01);
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

            QuantityAssert.HasValueAndUnit(metadataProvider.GetQuantity(obj, c => c.Size), 14.5, MassUnit.Ounce, 0.01);
        });
    }
}
