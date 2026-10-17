using System;
using System.Linq;

using NUnit.Framework;

using UnitsNet.Metadata.Tests.TestData;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests;

[TestFixture]
public class UnitChoicesTests
{
    private static QuantityMetadata Metadata(string property) => new Box().GetObjectMetadata()[property];

    [Test]
    public void ForListsTheUnitAndItsAllowedConversions()
    {
        var choices = UnitChoices.For(Metadata(nameof(Box.Weight)));

        Assert.Multiple(() =>
        {
            Assert.That(
                choices.Select(c => c.Id),
                Is.EquivalentTo(new[] { "Mass.Kilogram", "Mass.Gram", "Mass.Pound" }));
            Assert.That(choices.Select(c => c.Symbol), Is.EquivalentTo(new[] { "kg", "g", "lb" }));
            Assert.That(choices.Single(c => c.Id == "Mass.Pound").Name, Is.EqualTo("Pound"));
        });
    }

    [Test]
    public void ForListsEveryUnitWithoutAnAllowList()
    {
        var choices = UnitChoices.For(Metadata(nameof(Box.Width)));

        Assert.That(choices, Has.Count.EqualTo(Length.Info.UnitInfos.Length));
        Assert.That(choices.Select(c => c.Id), Does.Contain("Length.Meter").And.Contain("Length.Foot"));
    }

    [Test]
    public void ForWithoutMetadataIsEmpty()
    {
        Assert.That(UnitChoices.For(null), Is.Empty);
    }

    [TestCase("Mass.Pound")]
    [TestCase("mass.pound")]
    [TestCase("Pound")]
    [TestCase("pound")]
    [TestCase("lb")]
    public void ResolveMatchesIdNameOrSymbol(string name)
    {
        Assert.That(UnitChoices.Resolve(Metadata(nameof(Box.Weight)), name), Is.EqualTo(MassUnit.Pound));
    }

    [Test]
    public void ResolveIncludesTheDeclaredUnit()
    {
        Assert.That(UnitChoices.Resolve(Metadata(nameof(Box.Weight)), "kg"), Is.EqualTo(MassUnit.Kilogram));
    }

    [TestCase("LB")] // symbols are case sensitive
    [TestCase("t")] // a mass unit, but not one Weight allows
    [TestCase("m")] // not a mass unit
    public void ResolveRejectsUnavailableUnits(string name)
    {
        Assert.Throws<ArgumentException>(() => UnitChoices.Resolve(Metadata(nameof(Box.Weight)), name));
    }

    [Test]
    public void ResolveNullIsNull()
    {
        Assert.That(UnitChoices.Resolve(Metadata(nameof(Box.Weight)), null), Is.Null);
    }

    [Test]
    public void ResolveWithoutMetadataRejectsEverything()
    {
        Assert.Throws<ArgumentException>(() => UnitChoices.Resolve(null, "kg"));
    }

    [Test]
    public void Symbol()
    {
        var weight = Metadata(nameof(Box.Weight));

        Assert.Multiple(() =>
        {
            Assert.That(UnitChoices.Symbol(weight, MassUnit.Pound), Is.EqualTo("lb"));
            Assert.That(UnitChoices.Symbol(weight, null), Is.EqualTo("kg"));
            Assert.That(UnitChoices.Symbol(null, null), Is.EqualTo(""));
        });
    }

    [Test]
    public void SymbolOfAnUnavailableUnitThrows()
    {
        Assert.Throws<InvalidOperationException>(
            () => UnitChoices.Symbol(Metadata(nameof(Box.Weight)), MassUnit.Tonne));
    }
}
