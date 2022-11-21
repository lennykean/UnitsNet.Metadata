﻿using System;
using System.Globalization;
using System.Text.Json.Serialization;

using HondataDotNet.Datalog.Core.Annotations;
using HondataDotNet.Datalog.Core.Utils;
using UnitsNet;

namespace HondataDotNet.Datalog.Core.Metadata
{
    public class QuantityMetadata
    {
        public QuantityMetadata(QuantityAttribute metadataAttribute, CultureInfo? culture = null)
        {
            UnitInfo = metadataAttribute.UnitInfo;
            QuantityInfo = metadataAttribute.QuantityInfo;
            Unit = UnitInfo == null || QuantityInfo == null ? null : SimpleCache<Enum, UnitMetadataFull>.Instance.GetOrAdd(UnitInfo.Value, () => new UnitMetadataFull(UnitInfo, QuantityInfo, culture));
        }

        [JsonIgnore]
        public UnitInfo? UnitInfo { get; }
        [JsonIgnore]
        public QuantityInfo? QuantityInfo { get; }
        public UnitMetadataFull? Unit { get; }
    }
}