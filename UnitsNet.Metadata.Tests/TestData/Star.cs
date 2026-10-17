using UnitsNet.Metadata.Annotations;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.TestData;

public class Star
{
    public string? Name { get; set; }
    public int Number { get; set; }

    [Quantity(MassUnit.SolarMass)]
    public double Mass { get; set; }

    [Quantity(LengthUnit.Kilometer)]
    public double Radius { get; set; }

    [Quantity(LengthUnit.Parsec)]
    public double Distance { get; set; }

    [Quantity(LuminosityUnit.Petawatt)]
    public double Luminosity { get; set; }
}
