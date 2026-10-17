using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace UnitsNet.Metadata;

/// <summary>
/// Quantity metadata that can be cloned with overrides and validated.
/// </summary>
/// <typeparam name="TMetadata">The concrete metadata type</typeparam>
public interface IMetadata<TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    /// <summary>
    /// Creates a copy of the metadata, optionally replacing some of its values.
    /// </summary>
    /// <param name="overrideProperty">The property to use instead of the current one</param>
    /// <param name="overrideConversions">The conversions to use instead of the current ones</param>
    /// <param name="overrideUnit">The unit to use instead of the current one</param>
    /// <param name="overrideCulture">The culture to use instead of the current one</param>
    TMetadata Clone(
        PropertyInfo? overrideProperty = null,
        IEnumerable<UnitMetadataBasic>? overrideConversions = null,
        UnitMetadata? overrideUnit = null,
        CultureInfo? overrideCulture = null);
    /// <summary>
    /// Checks that the metadata is valid.
    /// </summary>
    void Validate();
}
