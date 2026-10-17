using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitsNet.Metadata;

/// <summary>
/// The units a quantity property can be shown in.
/// </summary>
public static class UnitChoices
{
    /// <summary>
    /// Gets the declared unit and allowed conversions of a quantity property.
    /// </summary>
    /// <param name="quantity">The quantity property's metadata</param>
    public static IReadOnlyList<UnitChoice> For(QuantityMetadata? quantity)
        => Units(quantity).Select(unit => new UnitChoice(Id(unit), unit.UnitInfo.Name, unit.Abbriviation)).ToArray();

    /// <summary>
    /// Gets the unit that matches an id, name or symbol, or <see langword="null"/> when the name is null.
    /// </summary>
    /// <param name="quantity">The quantity property's metadata</param>
    /// <param name="name">The unit id, name or symbol</param>
    public static Enum? Resolve(QuantityMetadata? quantity, string? name)
    {
        if (name is null)
        {
            return null;
        }

        var matches = Units(quantity).Where(unit =>
            string.Equals(Id(unit), name, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(unit.UnitInfo.Name, name, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(unit.Abbriviation, name, StringComparison.Ordinal)).ToArray();
        return matches.Length == 1
            ? matches[0].UnitInfo.Value
            : throw new ArgumentException(
                $"Unit '{name}' is unavailable or ambiguous. Use a unit id from the value's unit choices.",
                nameof(name));
    }

    /// <summary>
    /// Gets the symbol of a unit, or of the declared unit when the unit is null.
    /// </summary>
    /// <param name="quantity">The quantity property's metadata</param>
    /// <param name="unit">The unit</param>
    public static string Symbol(QuantityMetadata? quantity, Enum? unit)
        => unit is null
            ? quantity?.Unit?.Abbriviation ?? ""
            : Units(quantity).Single(candidate => candidate.UnitInfo.Value.Equals(unit)).Abbriviation;

    private static string Id(UnitMetadataBasic unit) => unit.QuantityType.QuantityInfo.Name + "." + unit.UnitInfo.Name;

    private static IEnumerable<UnitMetadataBasic> Units(QuantityMetadata? quantity)
        => quantity?.Unit is { } source
            ? quantity.Conversions
                .Append(UnitMetadataBasic.FromUnitInfo(source.UnitInfo, source.QuantityType.QuantityInfo))
                .OfType<UnitMetadataBasic>()
                .GroupBy(unit => unit.UnitInfo.Value).Select(group => group.First())
            : Enumerable.Empty<UnitMetadataBasic>();
}
