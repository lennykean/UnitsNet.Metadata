using UnitsNet.Metadata.Annotations;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.TestData;

public sealed class AerospaceVehicleTelemetry : VehicleTelemetry
{
    [Quantity(PressureUnit.Pascal)]
    public double PressureAltitude { get; set; }

    public override sealed double Efficency { get => base.Efficency; set => base.Efficency = value; }
    public override sealed double FuelLevel { get => base.FuelLevel; set => base.FuelLevel = value; }
    public override sealed double Speed { get => base.Speed; set => base.Speed = value; }
}
