using Microsoft.VisualStudio.TestPlatform.ObjectModel;

using NUnit.Framework;

using UnitsNet.Dataframes.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Dataframes.Tests.DataframeExtensions;

[TestFixture]
public class GetQuantity
{
    [TestCase(TestName = "{c} (gets the quantity)")]
    public void GetQuantityTest()
    {
        var box = new Box
        {
            Width = 1,
            Height = 2,
            Depth = 3,
            Weight = 4,
            Items = 5
        };

        var width = box.GetQuantity<Box, Length>("Width");
        var height = box.GetQuantity<Box, Length>(b => b.Height);
        var depth = box.GetQuantity("Depth");
        var weight = box.GetQuantity(b => b.Weight);
        var volume = box.GetQuantity(b => b.Volume);
        var items = box.GetQuantity(b => b.Items);

        Assert.Multiple(() =>
        {
            Assert.That(width, Has
                .Property(nameof(IQuantity.Value)).EqualTo(1).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(height, Has
                .Property(nameof(IQuantity.Value)).EqualTo(2).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(depth, Has
                .Property(nameof(IQuantity.Value)).EqualTo(3).And
                .Property(nameof(IQuantity.Unit)).EqualTo(LengthUnit.Meter));
            Assert.That(weight, Has
                .Property(nameof(IQuantity.Value)).EqualTo(4).And
                .Property(nameof(IQuantity.Unit)).EqualTo(MassUnit.Kilogram));
            Assert.That(items, Has
                .Property(nameof(IQuantity.Value)).EqualTo(5).And
                .Property(nameof(IQuantity.Unit)).EqualTo(ScalarUnit.Amount));
            Assert.That(volume, Has
                .Property(nameof(IQuantity.Value)).EqualTo(6).And
                .Property(nameof(IQuantity.Unit)).EqualTo(VolumeUnit.CubicMeter));
        });
    }

    [TestCase(TestName = "{c} (throws exception on missing metadata)")]
    public void GetQuantityMissingMetadataTest()
    {
        var box = new Box
        {
            SerialNumber = 1,
            Priority = 2,
        };

        Assert.Multiple(() =>
        {
            Assert.That(() => box.GetQuantity(b => b.SerialNumber),
                Throws.InvalidOperationException.With.Message.EqualTo("Unit metadata does not exist for Box.SerialNumber."));
            Assert.That(() => box.GetQuantity("Priority"),
                Throws.InvalidOperationException.With.Message.EqualTo("Unit metadata does not exist for Box.Priority."));
        });
    }

    [TestCase(TestName = "{c} (throws exception on invalid datatype)")]
    public void GetQuantityInvalidDatatypeTest()
    {
        var box = new Box
        {
            SerialNumber = 1,
            Priority = 2,
        };

        Assert.Multiple(() =>
        {
            Assert.That(() => box.GetQuantity("Data"),
                Throws.InvalidOperationException.With.Message.Match("(.*) type of (.*) is not compatible with (.*)"));
        });
    }
}