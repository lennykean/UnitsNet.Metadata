using System;

using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata;

/// <summary>
/// The basic metadata of a quantity type.
/// </summary>
public sealed class QuantityTypeMetadataBasic : QuantityTypeMetadataBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuantityTypeMetadataBasic"/> class.
    /// </summary>
    /// <param name="quantityInfo">The UnitsNet information of the quantity type</param>
    /// <param name="name">The name of the quantity type</param>
    /// <param name="displayName">The human-readable name of the quantity type</param>
    public QuantityTypeMetadataBasic(
        QuantityInfo quantityInfo,
        string name,
        string displayName) : base(quantityInfo, name, displayName)
    {
    }

    /// <summary>
    /// Gets the basic metadata of a quantity type.
    /// </summary>
    /// <param name="quantityInfo">The UnitsNet information of the quantity type</param>
    public static QuantityTypeMetadataBasic? FromQuantityInfo(QuantityInfo quantityInfo)
    {
        var cache = EphemeralValueCache<Type, QuantityTypeMetadataBasic>.GlobalInstance;
        return cache.GetOrAdd(quantityInfo.QuantityType, _ =>
        {
            var name = quantityInfo.Name;
            var displayName = GetDisplayName(quantityInfo);

            return new(quantityInfo, name, displayName);
        });
    }
}
