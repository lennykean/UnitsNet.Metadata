# UnitsNet.Metadata

UnitsNet.Metadata is a .NET Standard library that supplements the UnitsNet library, which provides a type-safe way to work with units of measure in .NET. The metadata library adds attributes that enable developers to declaratively specify units of measure for properties and define conversions between different units.

The library also includes an extensions API that can automatically convert properties with the metadata attributes to `IQuantity` objects, which represent the combination of a value and its unit of measure in the UnitsNet library. Additionally, the extensions API can perform dynamic unit conversions on these properties.

## Attributes
### QuantityAttribute
`QuantityAttribute` can be applied to properties to specify the unit type associated with the property. For example, a property that represents a yaw sensor can be annotated to indicate that the unit type is rotational speed, specifically degrees per second:

```csharp
[Quantity(RotationalSpeedUnit.DegreePerSecond)]
public double Yaw { get; set; };
```

## AllowUnitConversionAttribute
By default, the `ConvertQuantity` extension method can be used to convert a quantity to any UnitsNet built-in unit where a conversion is defined between them.  `AllowUnitConversionAttribute` can be applied to to limit the allowed unit conversions. For example, if only Fahrenheit and Celsius are acceptable temperature units, the following attributes can be applied:

```csharp
[Quantity(RotationalSpeedUnit.DegreeCelsius)]
[AllowUnitConversion(TemperatureUnit.DegreesFahrenheit)]
public double Temperature { get; set; }
```

If you are using custom units, `ConvertQuantity` will not allow any unit conversions by default. So `AllowUnitConversionAttribute` must be used to indicate any conversions.

## Extensions

### GetQuantity
The `GetQuantity` extension method returns an `IQuantity` object for a property based on the attributes applied to the property, and the property value.

```csharp
// Define a temperature property in Celsius
[Quantity(TemperatureUnit.DegreeCelsius)]
public double Temperature { get; set; }
...

// Set the temperature property
obj.Temperature = 30.0;

// Get the quantity of the temperature property
var tempQuantity = obj.GetQuantity(o => o.Temperature);

Console.WriteLine(tempQuantity); // 30°C
Console.WriteLine(tempQuantity.Value); // 30.0
Console.WriteLine(tempQuantity.Unit); // TemperatureUnit.DegreeCelsius
```

### ConvertQuantity
The ConvertQuantity extension method allows for the conversion of a property to any UnitsNet built-in unit that has a conversion defined. This method can be used on any property that has a QuantityAttribute applied to it. The method takes an optional IFormatProvider parameter to control the formatting of the output.

The following examples demonstrate how to use ConvertQuantity:

```csharp
// Set the temperature property
obj.Temperature = 30.0;

// Get the temperature property, converted to Fahrenheit
var temperatureInFahrenheitQuantity = temperature.ConvertQuantity(TemperatureUnit.DegreeFahrenheit);

Console.WriteLine(tempQuantity); // 86°F
```

## Metadata
UnitsNet.Metadata provides a set of metadata classes to represent the unit metadata defined on a given type.

* ObjectMetadata: Contains metadata for each property in the object
* QuantityMetadata: Contains metadata for a quantity, like the property, unit, and allowed unit conversions. 
* QuantityTypeMetadata: Contains metadata for a quantity type, like name. 
* UnitMetadata: Contains metadata for a unit, like the unit type and unit name
