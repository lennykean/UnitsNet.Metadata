# UnitsNet.Metadata

[![.github/workflows/build.yml](https://github.com/lennykean/UnitsNet.Metadata/actions/workflows/build.yml/badge.svg)](https://github.com/lennykean/UnitsNet.Metadata/actions/workflows/build.yml)
[![.github/workflows/publish.yml](https://github.com/lennykean/UnitsNet.Metadata/actions/workflows/publish.yml/badge.svg)](https://github.com/lennykean/UnitsNet.Metadata/actions/workflows/publish.yml)
[![UnitsNet.Metadata](https://img.shields.io/nuget/v/UnitsNet.Metadata.svg)](https://www.nuget.org/packages/UnitsNet.Metadata/)

UnitsNet.Metadata extends [UnitsNet](https://github.com/angularsen/UnitsNet) with unit metadata for object properties. Declare the unit of each property with annotations, fluent syntax or dynamic proxies, then read, convert and set property values as quantities.

## Features

- **Annotations**: Decorate class properties with units and allowed conversions using attributes.
- **Fluent syntax**: Configure metadata for properties and allowed conversions using a fluent interface.
- **Conversion methods**: Get, convert and set property values in other units using extension methods.
- **Dynamic proxies**: Create proxies of your objects that read and write properties in other units.
- **Metadata objects**: Get the unit, allowed conversions and quantity type of each property.
- **Custom units and attributes**: Use custom units and extend the metadata attributes.

## Getting Started

### Installation

Install UnitsNet.Metadata through NuGet:

```sh
dotnet add package UnitsNet.Metadata
```

### Annotations

Decorate class properties with their units using annotations.

```csharp
using UnitsNet.Metadata.Annotations;
using UnitsNet.Units;

public class Box
{
    [Quantity(LengthUnit.Meter)]
    public double Width { get; set; }

    [Quantity(LengthUnit.Meter)]
    public double Height { get; set; }

    [Quantity(MassUnit.Kilogram)]
    [AllowUnitConversion(MassUnit.Gram)]
    public double Weight { get; set; }
}
```

#### Allowed Conversions

By default, a property can be converted to any unit of its quantity type. Use the `AllowUnitConversion` attribute to limit a property to specific units.

## Retrieving Quantities

The `GetQuantity` method retrieves quantities as `IQuantity` objects. This can be helpful for obtaining the properties of an object in their original unit of measurement.

```csharp
using UnitsNet.Metadata;

var obj = new Box { Width = 2.5, Height = 3.5, Weight = 4.5 };

IQuantity width = obj.GetQuantity("Width");        // Retrieves 2.5 in meters
IQuantity height = obj.GetQuantity(b => b.Height); // Retrieves 3.5 in meters
IQuantity weight = obj.GetQuantity(b => b.Weight); // Retrieves 4.5 in kilograms
```

In this example, `GetQuantity` returns `IQuantity` objects representing the dimensions and weight of the box in the units they were initially set (meters for dimensions and kilograms for weight).

## Conversion Methods

The extension methods convert property values to other units, and set property values from other units, converting them to the declared unit.

### Convert Between Units

Use the `ConvertQuantity` method to convert properties into different units.

```csharp
using UnitsNet.Metadata;
using UnitsNet.Units;

var obj = new Box { Width = 2.5, Height = 3.5, Weight = 4.5 };

double widthInCentimeters = obj.ConvertQuantity("Width", LengthUnit.Centimeter);        // 250
double heightInCentimeters = obj.ConvertQuantity(b => b.Height, LengthUnit.Centimeter); // 350
double weightInGrams = obj.ConvertQuantity(b => b.Weight, MassUnit.Gram);               // 4500
```

### Set Properties in Different Units

The `SetQuantity` method sets a property from a quantity in any allowed unit, converting it to the declared unit of the property.

```csharp
using UnitsNet.Metadata;
using UnitsNet.Units;

var obj = new Box();

obj.SetQuantity("Width", Length.FromCentimeters(450));       // obj.Width == 4.5
obj.SetQuantity(b => b.Height, Length.FromCentimeters(625)); // obj.Height == 6.25
obj.SetQuantity(b => b.Weight, Mass.FromGrams(7800));       // obj.Weight == 7.8
```

Width and Height are set in centimeters and stored in meters, and Weight is set in grams and stored in kilograms.

## Fluent Syntax

UnitsNet.Metadata.Fluent configures metadata in code instead of with attributes. Install it through NuGet:

```sh
dotnet add package UnitsNet.Metadata.Fluent
```

The fluent interface sets the unit of each property and the conversions it allows:

```csharp
using UnitsNet.Metadata;
using UnitsNet.Metadata.Fluent;
using UnitsNet.Units;

var metadataProvider = MetadataBuilder
    .CreateFor<Box>()
    .With("Width", asUnit: LengthUnit.Meter)
        .WithAllowedConversion(to: LengthUnit.Millimeter)
        .WithAllowedConversion(to: LengthUnit.Centimeter)
    .With(b => b.Height, asUnit: LengthUnit.Meter)
        .WithAllowedConversion(to: LengthUnit.Millimeter)
        .WithAllowedConversion(to: LengthUnit.Centimeter)
    .Build(global: false);

var box = new Box();
var metadata = metadataProvider.GetObjectMetadata(box);
```

This declares that `Width` and `Height` are measured in meters and can be converted to millimeters and centimeters.

The `global` parameter of `Build` controls where the metadata applies. With `false`, as here, it is only available through the returned `metadataProvider`. With `true`, it applies everywhere, like annotations.

## Dynamic Proxies

UnitsNet.Metadata.DynamicProxy wraps your objects in proxies that read and write their properties in other units, without changing the units of the original objects. Install it through NuGet:

```sh
dotnet add package UnitsNet.Metadata.DynamicProxy
```

```csharp
using UnitsNet.Metadata.DynamicProxy;
using UnitsNet.Units;

var boxes = new[]
{
    new Box { Width = 1, Height = 2, Weight = 4 },
    new Box { Width = 2, Height = 4, Weight = 8 },
    new Box { Width = 4, Height = 8, Weight = 16 },
};

// Create dynamic proxies
var proxies = boxes.AsDynamicProxies()
    .WithConversion(b => b.Width, LengthUnit.Centimeter)
    .WithConversion(b => b.Height, LengthUnit.Centimeter)
    .WithConversion(b => b.Weight, MassUnit.Gram)
    .Build();

// Select the first proxy object
var proxy = proxies.First();

// The properties of the proxy will be in the converted units
double width = proxy.Width;   // 100, as it is converted to centimeters
double height = proxy.Height; // 200, as it is converted to centimeters

// Set values in the proxy, and it will be back-converted
proxy.Width = 200;  // boxes[0].Width will now be 2 meters
proxy.Height = 400; // boxes[0].Height will now be 4 meters
```

The proxies read `Width` and `Height` in centimeters and `Weight` in grams. Values set through a proxy are converted back to the units of the underlying object.

## Metadata Objects

Once the metadata is configured, `GetObjectMetadata` returns a description of each property.

```csharp
using UnitsNet.Metadata;

var box = new Box();
var metadata = box.GetObjectMetadata();
```

For the `Box` class, it returns:

```json
{
  "Width": {
    "FieldName": "Width",
    "Unit": {
      "QuantityType": {
        "BaseUnit": {
          "QuantityType": {
            "Name": "Length",
            "DisplayName": "length"
          },
          "Name": "Meter",
          "Value": 21,
          "DisplayName": "meters",
          "Abbreviation": "m"
        },
        "Name": "Length",
        "DisplayName": "length"
      },
      "Name": "Meter",
      "Value": 21,
      "DisplayName": "meters",
      "Abbreviation": "m"
    },
    "Conversions": [ ... /* all default length conversions */ ]
  },
  "Height": {
    "FieldName": "Height",
    ...
  },
  "Weight": {
    "FieldName": "Weight",
    ...
    "Conversions": [ // only the conversions explicitly defined
      {
        "QuantityType": {
          "Name": "Mass",
          "DisplayName": "mass"
        },
        "Name": "Kilogram",
        "Value": 8,
        "DisplayName": "kilograms",
        "Abbreviation": "kg"
      },
      {
        "QuantityType": {
          "Name": "Mass",
          "DisplayName": "mass"
        },
        "Name": "Gram",
        "Value": 6,
        "DisplayName": "grams",
        "Abbreviation": "g"
      }
    ]
  }
}
```

Each property has its `FieldName`, the `Unit` it is measured in, and the `Conversions` it allows. `Weight` only allows the units it declares, so only kilograms and grams are listed.

## License

UnitsNet.Metadata is released under the [MIT License](LICENSE).
