using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using Humanizer;

namespace UnitsNet.Metadata;

/// <summary>
/// The base of quantity type metadata.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class QuantityTypeMetadataBase
{
    private protected QuantityTypeMetadataBase(QuantityInfo quantityInfo, string name, string displayName)
    {
        QuantityInfo = quantityInfo;
        Name = name;
        DisplayName = displayName;
    }

    /// <summary>
    /// Gets the UnitsNet information of the quantity type.
    /// </summary>
    [JsonIgnore, IgnoreDataMember]
    public QuantityInfo QuantityInfo { get; }

    /// <summary>
    /// Gets the name of the quantity type.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the human-readable name of the quantity type.
    /// </summary>
    public string DisplayName { get; }

    private protected static string GetDisplayName(QuantityInfo quantityInfo)
    {
        return quantityInfo.Name.Humanize(LetterCasing.LowerCase);
    }
}
