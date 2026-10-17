using System.Linq;

using FizzWare.NBuilder;

using NUnit.Framework;

using UnitsNet.Metadata.DynamicProxy;
using UnitsNet.Metadata.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.DynamicProxies;

[TestFixture]
public class AsDynamicProxies
{
    [TestCase(TestName = "{c} (with conversions)")]
    public void WithConversionsTest()
    {
        var obj = new Box
        {
            Width = 1,
            Height = 2,
            Depth = 3,
            Weight = 4
        };

        var proxies = new[] { obj }.AsDynamicProxies()
            .WithConversion(b => b.Width, LengthUnit.Centimeter)
            .WithConversion(b => b.Height, LengthUnit.Centimeter)
            .WithConversion(b => b.Depth, LengthUnit.Centimeter)
            .WithConversion(b => b.Weight, MassUnit.Gram)
            .WithConversion(b => b.Volume, VolumeUnit.CubicDecimeter)
            .Build();

        var proxy = proxies.First();
        var metadata = proxy.GetObjectMetadata();
        var collectionMetadata = proxies.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            Assert.That(collectionMetadata, Is.EquivalentTo(metadata));
            Assert.That(metadata, Has.Count.EqualTo(6));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Width))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Centimeter));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Height))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Centimeter));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Depth))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(LengthUnit.Centimeter));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Weight))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(MassUnit.Gram));
            Assert.That(metadata, Has.ItemAt(nameof(Box.Volume))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(VolumeUnit.CubicDecimeter));

            Assert.That(proxy, Has.Property(nameof(Box.Width)).EqualTo(100));
            Assert.That(proxy, Has.Property(nameof(Box.Height)).EqualTo(200));
            Assert.That(proxy, Has.Property(nameof(Box.Depth)).EqualTo(300));
            Assert.That(proxy, Has.Property(nameof(Box.Weight)).EqualTo(4000));
            Assert.That(proxy, Has.Property(nameof(Box.Volume)).EqualTo(6000));
        });
    }

    [TestCase(TestName = "{c} (with setters)")]
    public void WithSettersTest()
    {
        var obj = new Box
        {
            Width = 1,
            Height = 2,
            Depth = 3,
            Weight = 4
        };

        var proxy = new[] { obj }.AsDynamicProxies()
            .WithConversion(b => b.Width, LengthUnit.Centimeter)
            .WithConversion(b => b.Height, LengthUnit.Centimeter)
            .WithConversion(b => b.Depth, LengthUnit.Centimeter)
            .WithConversion(b => b.Weight, MassUnit.Gram)
            .WithConversion(b => b.Volume, VolumeUnit.CubicDecimeter)
            .Build()
            .First();

        proxy.Width = 200;
        proxy.Height = 300;
        proxy.Depth = 400;
        proxy.Weight = 5000;

        Assert.Multiple(() =>
        {
            Assert.That(obj, Has.Property(nameof(Box.Width)).EqualTo(2));
            Assert.That(proxy, Has.Property(nameof(Box.Width)).EqualTo(200));
            Assert.That(obj, Has.Property(nameof(Box.Height)).EqualTo(3));
            Assert.That(proxy, Has.Property(nameof(Box.Height)).EqualTo(300));
            Assert.That(obj, Has.Property(nameof(Box.Depth)).EqualTo(4));
            Assert.That(proxy, Has.Property(nameof(Box.Depth)).EqualTo(400));
            Assert.That(obj, Has.Property(nameof(Box.Weight)).EqualTo(5));
            Assert.That(proxy, Has.Property(nameof(Box.Weight)).EqualTo(5000));
            Assert.That(obj, Has.Property(nameof(Box.Volume)).EqualTo(24));
            Assert.That(proxy, Has.Property(nameof(Box.Volume)).EqualTo(24000));
        });
    }

    [TestCase(TestName = "{c} (with missing metadata)")]
    public void WithMissingMetadataTest()
    {
        Assert.That(() =>
            Builder<Star>.CreateListOfSize(100).Build().AsDynamicProxies()
                .WithConversion(s => s.Number, ScalarUnit.Amount)
                .Build(),
            Throws.InvalidOperationException.With.Message.Match("Unit metadata does not exist for (.*)"));
    }

    [TestCase(TestName = "{c} (with non-virtual property conversions)")]
    public void WithNonVirtualPropertyConversionsTest()
    {
        Assert.Multiple(() =>
        {
            Assert.That(() =>
                Builder<Star>.CreateListOfSize(100).Build().AsDynamicProxies()
                    .WithConversion(s => s.Mass, MassUnit.SolarMass)
                    .WithConversion(b => b.Radius, LengthUnit.SolarRadius)
                    .WithConversion(b => b.Distance, LengthUnit.AstronomicalUnit)
                    .WithConversion(b => b.Luminosity, LuminosityUnit.SolarLuminosity)
                    .Build(),
                Throws.InvalidOperationException.With.Message
                    .Match("(.*) is non-virtual and cannot be converted to (.*)"));

            Assert.That(() =>
                Builder<TransmitterData>.CreateListOfSize(100).Build().AsDynamicProxies()
                    .WithConversion(b => b.Power, PowerUnit.Milliwatt)
                    .WithConversion(b => b.Frequency, FrequencyUnit.Kilohertz)
                    .WithConversion(b => b.Temperature, TemperatureUnit.DegreeCelsius)
                    .Build(),
                Throws.InvalidOperationException.With.Message
                    .Match("(.*) is non-virtual and cannot be converted to (.*)"));
        });
    }

    [TestCase(TestName = "{c} (with interface hoisted metadata)")]
    public void WithInterfaceHoistedMetadataTest()
    {
        var obj = new TransmitterData
        {
            Power = 2,
            Frequency = 3_000_000,
            Temperature = 100
        };

        var proxies = new[] { obj }.AsDynamicProxies()
            .WithConversion(b => b.Power, PowerUnit.Milliwatt)
            .WithConversion(b => b.Frequency, FrequencyUnit.Megahertz)
            .WithConversion(b => b.Temperature, TemperatureUnit.DegreeCelsius)
            .As<ITransmitterThermalData>()
            .Build();

        var proxy = proxies.First();
        var metadata = proxy.GetObjectMetadata();
        var collectionMetadata = proxies.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            Assert.That(collectionMetadata, Is.EquivalentTo(metadata));
            Assert.That(metadata, Has.ItemAt(nameof(ITransmitterThermalData.Power))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(PowerUnit.Milliwatt));
            Assert.That(metadata, Has.ItemAt(nameof(ITransmitterThermalData.Frequency))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(FrequencyUnit.Megahertz));
            Assert.That(metadata, Has.ItemAt(nameof(ITransmitterThermalData.Temperature))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(TemperatureUnit.DegreeCelsius));

            Assert.That(proxy, Has.Property(nameof(TransmitterData.Power)).EqualTo(2000).Within(0.01));
            Assert.That(proxy, Has.Property(nameof(TransmitterData.Frequency)).EqualTo(3).Within(0.01));
            Assert.That(proxy, Has.Property(nameof(TransmitterData.Temperature)).EqualTo(-173.15).Within(0.01));
        });
    }

    [TestCase(TestName = "{c} (with virtual hoisted metadata)")]
    public void WithVirtualHoistedMetadataTest()
    {
        var obj = new AerospaceVehicleTelemetry
        {
            Efficency = 2,
            Speed = 10
        };

        var proxies = new[] { obj }.AsDynamicProxies()
            .WithConversion(t => t.Efficency, FuelEfficiencyUnit.MilePerUsGallon)
            .WithConversion(t => t.Speed, SpeedUnit.MilePerHour)
            .As<VehicleTelemetry>()
            .Build();

        var proxy = proxies.First();
        var metadata = proxies.GetObjectMetadata();
        var collectionMetadata = proxies.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            Assert.That(collectionMetadata, Is.EquivalentTo(metadata));
            Assert.That(metadata, Has.ItemAt(nameof(VehicleTelemetry.Efficency))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(FuelEfficiencyUnit.MilePerUsGallon));
            Assert.That(metadata, Has.ItemAt(nameof(VehicleTelemetry.Speed))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(SpeedUnit.MilePerHour));

            Assert.That(proxy, Has.Property(nameof(MotorVehicleTelemetry.Efficency)).EqualTo(4.70).Within(0.01));
            Assert.That(proxy, Has.Property(nameof(MotorVehicleTelemetry.Speed)).EqualTo(6.21).Within(0.01));
        });
    }

    [TestCase(TestName = "{c} (with virtual property metadata)")]
    public void WithVirtualPropertyMetadataTest()
    {
        var obj = new MotorVehicleTelemetry
        {
            Efficency = 2,
            Speed = 10
        };

        var proxies = new[] { obj }.AsDynamicProxies()
            .WithConversion(t => t.Efficency, FuelEfficiencyUnit.MilePerUsGallon)
            .WithConversion(t => t.Speed, SpeedUnit.MilePerHour)
            .Build();

        var proxy = proxies.First();
        var metadata = proxy.GetObjectMetadata();
        var collectionMetadata = proxies.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            Assert.That(collectionMetadata, Is.EquivalentTo(metadata));
            Assert.That(metadata, Has.ItemAt(nameof(VehicleTelemetry.Efficency))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(FuelEfficiencyUnit.MilePerUsGallon));
            Assert.That(metadata, Has.ItemAt(nameof(VehicleTelemetry.Speed))
                .Property(nameof(QuantityMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(SpeedUnit.MilePerHour));

            Assert.That(proxy, Has.Property(nameof(MotorVehicleTelemetry.Efficency)).EqualTo(4.7).Within(0.01));
            Assert.That(proxy, Has.Property(nameof(MotorVehicleTelemetry.Speed)).EqualTo(6.21).Within(0.01));
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

        var proxies = new[] { obj }.AsDynamicProxies()
            .WithConversion(t => t.Horsepower, PowerUnit.Kilowatt)
            .WithConversion(t => t.Torque, TorqueUnit.NewtonMeter)
            .Build();

        var proxy = proxies.First();
        var metadata = proxy.GetObjectMetadata();
        var collectionMetadata = proxies.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            Assert.That(collectionMetadata, Is.EquivalentTo(metadata));
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Horsepower))
                .Property(nameof(DisplayMeasurementMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(PowerUnit.Kilowatt));
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Torque))
                .Property(nameof(DisplayMeasurementMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(TorqueUnit.NewtonMeter));
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Rpm))
                .Property(nameof(DisplayMeasurementMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(RotationalSpeedUnit.RevolutionPerMinute));

            QuantityAssert.HasValueAndUnit(
                obj.ConvertQuantity(d => d.Horsepower, to: PowerUnit.Kilowatt), 223.7, PowerUnit.Kilowatt, 0.01);
            QuantityAssert.HasValueAndUnit(
                obj.ConvertQuantity("Torque", to: TorqueUnit.NewtonMeter), 271.16, TorqueUnit.NewtonMeter, 0.01);
        });
    }

    [TestCase(TestName = "{c} (typed with custom attribute)")]
    public void TypedWithCustomAttributeTest()
    {
        var obj = new DynoData
        {
            Horsepower = 300,
            Torque = 200,
            Rpm = 6000
        };

        var proxies = new[] { obj }
            .AsDynamicProxies<DynoData, DisplayMeasurementAttribute, DisplayMeasurementMetadata>()
            .WithConversion(t => t.Horsepower, PowerUnit.Kilowatt)
            .WithConversion(t => t.Torque, TorqueUnit.NewtonMeter)
            .Build();

        var proxy = proxies.First();
        var metadata = proxy.GetObjectMetadata<DynoData, DisplayMeasurementAttribute, DisplayMeasurementMetadata>();
        var collectionMetadata = proxies.GetObjectMetadata();

        Assert.Multiple(() =>
        {
            Assert.That(collectionMetadata, Is.EquivalentTo(metadata));
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Horsepower))
                .Property(nameof(DisplayMeasurementMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(PowerUnit.Kilowatt).And
                .ItemAt(nameof(DynoData.Horsepower)).Property(nameof(DisplayMeasurementMetadata
                    .DisplayName)).EqualTo("Engine Horsepower"));
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Torque))
                .Property(nameof(DisplayMeasurementMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(TorqueUnit.NewtonMeter).And
                .ItemAt(nameof(DynoData.Torque)).Property(nameof(DisplayMeasurementMetadata
                    .DisplayName)).EqualTo("Engine Torque"));
            Assert.That(metadata, Has.ItemAt(nameof(DynoData.Rpm))
                .Property(nameof(DisplayMeasurementMetadata.Unit)).Property(nameof(UnitMetadata
                    .UnitInfo)).Property(nameof(UnitInfo.Value)).EqualTo(RotationalSpeedUnit.RevolutionPerMinute).And
                .ItemAt(nameof(DynoData.Rpm)).Property(nameof(DisplayMeasurementMetadata
                    .DisplayName)).EqualTo("Engine Speed"));

            QuantityAssert.HasValueAndUnit(
                obj.ConvertQuantity(d => d.Horsepower, to: PowerUnit.Kilowatt), 223.7, PowerUnit.Kilowatt, 0.01);
            QuantityAssert.HasValueAndUnit(
                obj.ConvertQuantity("Torque", to: TorqueUnit.NewtonMeter), 271.16, TorqueUnit.NewtonMeter, 0.01);
        });
    }
}
