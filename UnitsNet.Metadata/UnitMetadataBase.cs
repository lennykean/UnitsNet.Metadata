using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using Humanizer;

namespace UnitsNet.Metadata;

/// <summary>
/// The base of unit metadata.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class UnitMetadataBase
{
    private protected UnitMetadataBase(
        UnitInfo unitInfo,
        string name,
        int value,
        string displayName,
        string abbriviation)
    {
        UnitInfo = unitInfo;
        Name = name;
        Value = value;
        DisplayName = displayName;
        Abbriviation = abbriviation;
    }

    /// <summary>
    /// Gets the UnitsNet unit information.
    /// </summary>
    [JsonIgnore, IgnoreDataMember]
    public UnitInfo UnitInfo { get; }

    /// <summary>
    /// Gets the unit name.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Gets the numeric value of the unit enumeration.
    /// </summary>
    public int Value { get; }
    /// <summary>
    /// Gets the human-readable name of the unit.
    /// </summary>
    public string DisplayName { get; }
    /// <summary>
    /// Gets the unit abbreviation.
    /// </summary>
    public string Abbriviation { get; }

    private protected static string GetDisplayName(UnitInfo unitInfo)
    {
        return unitInfo.PluralName.Humanize(LetterCasing.LowerCase);
    }

    private protected static string GetAbbriviation(UnitInfo unitInfo, CultureInfo? culture)
    {
        return UnitAbbreviationsCache.Default.GetUnitAbbreviations(unitInfo, culture).FirstOrDefault() ?? "";
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is UnitMetadataBase other && UnitInfo.Value.Equals(other.UnitInfo.Value);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(UnitInfo.Value);
    }
}
