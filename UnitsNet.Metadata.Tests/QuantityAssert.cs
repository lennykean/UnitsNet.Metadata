using System;

using NUnit.Framework;

namespace UnitsNet.Metadata.Tests;

internal static class QuantityAssert
{
    internal static void HasValueAndUnit(
        IQuantity actual,
        double expectedValue,
        Enum expectedUnit,
        double tolerance = 0)
    {
        Assert.Multiple(() =>
        {
            Assert.That((double)actual.Value, Is.EqualTo(expectedValue).Within(tolerance));
            Assert.That(actual.Unit, Is.EqualTo(expectedUnit));
        });
    }
}
