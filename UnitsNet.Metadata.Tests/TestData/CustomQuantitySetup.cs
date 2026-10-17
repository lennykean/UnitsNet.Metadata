using System.Globalization;
using System.Runtime.CompilerServices;

using UnitsNet.Metadata.Tests.Compatibility;

namespace UnitsNet.Metadata.Tests.TestData;

internal static class CustomQuantitySetup
{
    [ModuleInitializer]
    internal static void Initialize()
    {
        UnitsNetSetup.ConfigureDefaults(builder => builder.WithAdditionalQuantities(
            new QuantityInfo[] { Coolness.Info, NumericCompatibility.CustomQuantity.Info }));

        var abbreviations = UnitAbbreviationsCache.Default;
        var culture = CultureInfo.InvariantCulture;
        abbreviations.MapUnitToDefaultAbbreviation(Coolness.Info[CoolnessUnit.Fonzie], culture, "Fz");
        abbreviations.MapUnitToDefaultAbbreviation(Coolness.Info[CoolnessUnit.MegaFonzie], culture, "mFz");
        abbreviations.MapUnitToDefaultAbbreviation(
            NumericCompatibility.CustomQuantity.Info[NumericCompatibility.CustomUnit.Token],
            culture,
            "tk");
    }
}
