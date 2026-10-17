using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.TestData;

public class DynoData
{
    [DisplayMeasurement(PowerUnit.MechanicalHorsepower, displayName: "Engine Horsepower")]
    public virtual double Horsepower { get; set; }

    [DisplayMeasurement(TorqueUnit.PoundForceFoot, displayName: "Engine Torque")]
    public virtual double Torque { get; set; }

    [DisplayMeasurement(RotationalSpeedUnit.RevolutionPerMinute, displayName: "Engine Speed")]
    public double Rpm { get; set; }
}
