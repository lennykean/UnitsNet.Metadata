using System.Globalization;
using System.Linq;

namespace UnitsNet.Metadata;

internal static class MetadataCultureExtensions
{
    internal static TMetadata WithCulture<TMetadata>(this TMetadata metadata, CultureInfo culture)
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        var unit = metadata.Unit;
        return metadata.Clone(
            overrideUnit: unit is null
                ? null
                : UnitMetadata.FromUnitInfo(unit.UnitInfo, unit.QuantityType.QuantityInfo, culture),
            overrideConversions: metadata.Conversions.Select(conversion =>
                UnitMetadataBasic.FromUnitInfo(conversion.UnitInfo, conversion.QuantityType.QuantityInfo, culture)!),
            overrideCulture: culture);
    }
}
