using System.Globalization;
using System.Linq;

using NUnit.Framework;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.DynamicProxy;
using UnitsNet.Metadata.Fluent;
using UnitsNet.Units;

namespace UnitsNet.Metadata.Tests.Compatibility;

[TestFixture]
[NonParallelizable]
public class CultureCompatibility
{
    [TestCase("en-US", "ru-RU")]
    [TestCase("ru-RU", "en-US")]
    public void AnnotationMetadataUsesRequestedCultureAtEveryLevel(string first, string second)
    {
        var reading = new Reading();
        var firstMetadata = reading.GetObjectMetadata(CultureInfo.GetCultureInfo(first))[nameof(Reading.Length)];
        var secondMetadata = reading.GetObjectMetadata(CultureInfo.GetCultureInfo(second))[nameof(Reading.Length)];
        Check(firstMetadata, first, LengthUnit.Meter);
        Check(secondMetadata, second, LengthUnit.Meter);
        Check(firstMetadata, first, LengthUnit.Meter);
    }

    [Test]
    public void DefaultCultureTracksCurrentCulture()
    {
        var previousCulture = CultureInfo.CurrentCulture;
        try
        {
            var reading = new Reading();
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Check(reading.GetObjectMetadata()[nameof(Reading.Length)], "en-US", LengthUnit.Meter);
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
            Check(reading.GetObjectMetadata()[nameof(Reading.Length)], "ru-RU", LengthUnit.Meter);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
        }
    }

    [Test]
    public void FluentBuildAndExplicitLookupLocalizeUnitsAndConversions()
    {
        var provider = MetadataBuilder.CreateFor<Reading>()
            .With(r => r.Length, LengthUnit.Meter)
            .Build(global: false, culture: CultureInfo.GetCultureInfo("ru-RU"));
        var reading = new Reading();
        var english = CultureInfo.GetCultureInfo("en-US");
        Check(provider.GetObjectMetadata(reading)[nameof(Reading.Length)], "ru-RU", LengthUnit.Meter);
        Check(provider.GetObjectMetadata(reading, english)[nameof(Reading.Length)], "en-US", LengthUnit.Meter);
        Check(provider.GetObjectMetadata(reading)[nameof(Reading.Length)], "ru-RU", LengthUnit.Meter);
    }

    [Test]
    public void ProxyMetadataCanBeReadInMoreThanOneCulture()
    {
        var proxy = new[] { new Reading() }.AsDynamicProxies()
            .WithConversion(r => r.Length, LengthUnit.Centimeter)
            .Build()
            .Single();
        var english = proxy.GetObjectMetadata(CultureInfo.GetCultureInfo("en-US"))[nameof(Reading.Length)];
        var russian = proxy.GetObjectMetadata(CultureInfo.GetCultureInfo("ru-RU"))[nameof(Reading.Length)];
        Check(english, "en-US", LengthUnit.Centimeter);
        Check(russian, "ru-RU", LengthUnit.Centimeter);
    }

    private static void Check(QuantityMetadata metadata, string cultureName, LengthUnit unit)
    {
        var culture = CultureInfo.GetCultureInfo(cultureName);
        string Abbreviation(LengthUnit value)
            => UnitAbbreviationsCache.Default.GetUnitAbbreviations(value, culture).First();

        Assert.Multiple(() =>
        {
            Assert.That(metadata.Unit!.Abbriviation, Is.EqualTo(Abbreviation(unit)));
            Assert.That(metadata.Unit.QuantityType.BaseUnit!.Abbriviation, Is.EqualTo(Abbreviation(LengthUnit.Meter)));
            foreach (var conversion in metadata.Conversions)
            {
                Assert.That(conversion.Abbriviation, Is.EqualTo(Abbreviation((LengthUnit)conversion.UnitInfo.Value)));
            }
        });
    }

    public class Reading
    {
        [Quantity(LengthUnit.Meter)]
        public virtual double Length { get; set; }
    }
}
