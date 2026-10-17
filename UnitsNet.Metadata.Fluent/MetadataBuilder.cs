
using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata.Fluent;

/// <summary>
/// The entry point for fluent metadata builders.
/// </summary>
public abstract class MetadataBuilder
{
    internal MetadataBuilder()
    {
    }

    /// <summary>
    /// Creates a metadata builder for the given object type and metadata types.
    /// </summary>
    /// <typeparam name="TObject">The type the metadata is built for</typeparam>
    /// <typeparam name="TMetadataAttribute">The metadata attribute type</typeparam>
    /// <typeparam name="TMetadata">The metadata type</typeparam>
    public static MetadataBuilder<TObject, TMetadataAttribute, TMetadata>
        CreateFor<TObject, TMetadataAttribute, TMetadata>()
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        return new();
    }

    /// <summary>
    /// Creates a metadata builder for the given object type using the default metadata types.
    /// </summary>
    /// <typeparam name="TObject">The type the metadata is built for</typeparam>
    public static MetadataBuilder<TObject> CreateFor<TObject>()
        where TObject : class
    {
        return new();
    }
}
