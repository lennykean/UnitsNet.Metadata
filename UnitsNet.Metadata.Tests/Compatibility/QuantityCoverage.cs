using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace UnitsNet.Metadata.Tests.Compatibility;

[TestFixture]
public class QuantityCoverage
{
    public static IEnumerable<TestCaseData> Quantities()
    {
        foreach (var quantity in Quantity.Infos)
        {
            yield return new TestCaseData(quantity).SetName("Metadata and conversions: " + quantity.Name);
        }
    }

    [TestCaseSource(nameof(Quantities))]
    public void AllUnitsCanBeDiscoveredAndConvertedToTheirBaseUnit(QuantityInfo quantity)
    {
        var baseMetadata = UnitMetadata.FromUnitInfo(quantity.BaseUnitInfo, quantity);
        foreach (var unit in quantity.UnitInfos)
        {
            Assert.Multiple(() =>
            {
                Assert.That(unit.Value.TryGetQuantityInfo(null, out var discoveredQuantity), Is.True, unit.Name);
                Assert.That(discoveredQuantity!.Name, Is.EqualTo(quantity.Name), unit.Name);
                Assert.That(unit.Value.TryGetUnitInfo(null, out var discoveredUnit), Is.True, unit.Name);
                Assert.That(discoveredUnit!.Value, Is.EqualTo(unit.Value));
            });

            var metadata = UnitMetadata.FromUnitInfo(unit, quantity);
            Assert.That(metadata.TryConvertQuantity(1.0, baseMetadata, out var converted), Is.True, unit.Name);
            var expected = (double)UnitConverter.Default.ConvertValue(1, unit.UnitKey, quantity.BaseUnitInfo.UnitKey);
            var tolerance = Math.Max(1, Math.Abs(expected)) * 1e-10;
            Assert.That((double)converted.Value, Is.EqualTo(expected).Within(tolerance), unit.Name);
        }
    }
}
