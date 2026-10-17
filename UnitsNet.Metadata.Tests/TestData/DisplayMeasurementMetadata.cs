using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace UnitsNet.Metadata.Tests.TestData;

public class DisplayMeasurementMetadata : QuantityMetadata, IMetadata<DisplayMeasurementMetadata>
{
    public DisplayMeasurementMetadata(
        PropertyInfo property,
        UnitMetadata? unit,
        IEnumerable<UnitMetadataBasic> conversions,
        string? displayName) : base(property, unit, conversions.ToList())
    {
        DisplayName = displayName;
    }

    public string? DisplayName { get; }

    public DisplayMeasurementMetadata Clone(
        PropertyInfo? overrideProperty = null,
        IEnumerable<UnitMetadataBasic>? overrideConversions = null,
        UnitMetadata? overrideUnit = null,
        CultureInfo? overrideCulture = null)
    {
        return new DisplayMeasurementMetadata(
            overrideProperty ?? Property,
            overrideUnit ?? Unit,
            overrideConversions ?? Conversions,
            DisplayName);
    }

    public override void Validate()
    {
        base.Validate();
        if (DisplayName is null)
        {
            throw new InvalidOperationException($"The {nameof(DisplayName)} property must be set.");
        }
    }
}
