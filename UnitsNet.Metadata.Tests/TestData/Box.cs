using UnitsNet.Metadata.Annotations;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.TestData;

public class Box
{
    public virtual int SerialNumber { get; set; }
    public virtual int Priority { get; set; }

    [Quantity(LengthUnit.Meter)]
    public virtual double Width { get; set; }

    [Quantity(LengthUnit.Meter)]
    public virtual double Height { get; set; }

    [Quantity(LengthUnit.Meter)]
    public virtual double Depth { get; set; }

    [Quantity(MassUnit.Kilogram)]
    [AllowUnitConversion(MassUnit.Gram)]
    [AllowUnitConversion(MassUnit.Pound)]
    public virtual double Weight { get; set; }

    [Quantity(ScalarUnit.Amount)]
    public virtual int Items { get; set; }

    [Quantity(VolumeUnit.CubicMeter)]
    [AllowUnitConversion(VolumeUnit.CubicDecimeter)]
    [AllowUnitConversion(VolumeUnit.CubicInch)]
    public virtual double Volume => Width * Height * Depth;
}
