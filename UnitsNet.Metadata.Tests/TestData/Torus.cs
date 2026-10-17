using System;

namespace UnitsNet.Metadata.Tests.TestData;

public class Torus
{
    public double MajorRadius { get; set; }
    public double MinorRadius { get; set; }
    public double MinorArea => Math.PI * MinorRadius * MinorRadius;
    public double MajorCircumference => 2 * Math.PI * MajorRadius;
    public double Volume => MinorArea * MajorCircumference;
}
