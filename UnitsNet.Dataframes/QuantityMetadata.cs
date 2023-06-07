﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes;

public class QuantityMetadata : DataframeMetadata<QuantityAttribute, QuantityMetadata>.IClonableMetadata
{
    public QuantityMetadata(PropertyInfo property, UnitMetadata? unit, IList<UnitMetadataBasic> conversions)
    {
        Property = property;
        Unit = unit;
        Conversions = new(conversions);
    }

    [JsonIgnore, IgnoreDataMember]
    public PropertyInfo Property { get; }
    public string FieldName => Property.Name;
    public UnitMetadata? Unit { get; }
    public ReadOnlyCollection<UnitMetadataBasic> Conversions { get; }

    QuantityMetadata DataframeMetadata<QuantityAttribute, QuantityMetadata>.IClonableMetadata.Clone(
        PropertyInfo? overrideProperty,
        IEnumerable<UnitMetadataBasic>? overrideConversions,
        UnitMetadata? overrideUnit,
        CultureInfo? overrideCulture)
    {
        return new QuantityMetadata(overrideProperty ?? Property, overrideUnit ?? Unit, (overrideConversions ?? Conversions).ToList());
    }
}