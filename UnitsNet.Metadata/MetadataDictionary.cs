using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnitsNet.Metadata;

internal sealed class MetadataDictionary<TMetadata> : IReadOnlyDictionary<string, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    private readonly IEnumerable<TMetadata> _metadatas;

    internal MetadataDictionary(IEnumerable<TMetadata> metadatas)
    {
        _metadatas = metadatas;
    }

    public IEnumerable<string> Keys => _metadatas.Select(metadata => metadata.FieldName).Distinct();

    public IEnumerable<TMetadata> Values => _metadatas;

    public int Count => _metadatas.Count();

    public TMetadata this[PropertyInfo key] => _metadatas.FirstOrDefault(metadata => metadata.Property == key);

    public TMetadata this[string key] => _metadatas.FirstOrDefault(metadata => metadata.FieldName == key);

    public bool ContainsKey(string key)
    {
        return _metadatas.Any(metadata => metadata.FieldName == key);
    }

    public bool ContainsKey(PropertyInfo key)
    {
        return _metadatas.Any(metadata => metadata.Property == key);
    }

    public bool TryGetValue(string key, out TMetadata value)
    {
        value = default!;
        if (!ContainsKey(key))
        {
            return false;
        }

        value = this[key];
        return true;
    }

    public bool TryGetValue(PropertyInfo key, out TMetadata value)
    {
        value = default!;
        if (!ContainsKey(key))
        {
            return false;
        }

        value = this[key];
        return true;
    }

    public IEnumerator<KeyValuePair<string, TMetadata>> GetEnumerator()
    {
        return _metadatas.Select(metadata => new KeyValuePair<string, TMetadata>(metadata.FieldName, metadata))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
