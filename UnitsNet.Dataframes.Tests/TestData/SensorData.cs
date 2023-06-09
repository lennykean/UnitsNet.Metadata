﻿using UnitsNet.Dataframes.Attributes;
using UnitsNet.Units;

namespace UnitsNet.Dataframes.Tests.TestData;

public class SensorData : ISensorData
{
    [Quantity(PowerUnit.Watt)]
    public double Power { get; set; }
    [Quantity(FrequencyUnit.Hertz)]
    public double Frequency { get; set; }
    [Quantity(TemperatureUnit.Kelvin)]
    public double Temperature { get; set; }
}