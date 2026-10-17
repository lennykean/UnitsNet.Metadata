using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.Reflection;
using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata.DynamicProxy;

internal sealed class DynamicProxyMetadataProvider<TMetadataAttribute, TMetadata>
    : IMetadataProvider<TMetadataAttribute, TMetadata>
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    private readonly IMetadataProvider<TMetadataAttribute, TMetadata> _baseMetadataProvider;
    private readonly ConcurrentDictionary<PropertyInfo, TMetadata> _dynamicMetadata;

    public DynamicProxyMetadataProvider(
        IMetadataProvider<TMetadataAttribute, TMetadata> baseMetadataProvider,
        IEnumerable<TMetadata>? dynamicMetadata = null)
    {
        _baseMetadataProvider = baseMetadataProvider;
        _dynamicMetadata = new(
            dynamicMetadata?.ToDictionary(k => k.Property, v => v) ?? new(), new DeclaringTypePropertyComparer());
    }

    public bool TryGetMetadata(
        PropertyInfo property,
        [NotNullWhen(true)] out TMetadata? metadata,
        CultureInfo? culture = null)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        if (_dynamicMetadata.TryGetValue(property, out metadata) && metadata.Unit is not null)
        {
            return true;
        }

        if (_baseMetadataProvider.TryGetMetadata(property, out metadata) && metadata?.Unit is not null)
        {
            return true;
        }

        return false;
    }

    public bool TryGetBaseMetadata(PropertyInfo property, [NotNullWhen(true)] out TMetadata? metadata)
    {
        return _baseMetadataProvider.TryGetMetadata(property, out metadata);
    }

    public void AddConversion(PropertyInfo property, Enum unit, CultureInfo? culture = null)
    {
        var (_, toMetadata) = property.GetConversionMetadatas(to: unit, this, culture);

        if (!_baseMetadataProvider.TryGetMetadata(property, out var innerMetadata) || innerMetadata is null ||
            innerMetadata.Unit is null)
        {
            throw new InvalidOperationException($"No metadata found for property {property}.");
        }

        var convertBack = UnitMetadataBasic.FromUnitInfo(
            innerMetadata.Unit.UnitInfo,
            innerMetadata.Unit.QuantityType.QuantityInfo,
            culture)!;
        var conversions = innerMetadata.Conversions.Append(convertBack);

        var metadata = innerMetadata.Clone(
            overrideProperty: property,
            overrideConversions: conversions,
            overrideUnit: toMetadata,
            overrideCulture: culture);

        _dynamicMetadata.AddOrUpdate(property, _ => metadata, (_, _) => metadata);
    }

    public void ValidateMetadata(PropertyInfo property)
    {
        if (!TryGetMetadata(property, out var metadata))
        {
            return;
        }

        metadata.Validate();

        if (_baseMetadataProvider.TryGetMetadata(metadata.Property, out var baseMetadata) &&
            metadata.Unit is not null && metadata.Unit.UnitInfo != baseMetadata.Unit?.UnitInfo &&
            metadata.Property.GetMethod is not null && (!metadata.Property.GetMethod.IsAbstract &&
                !metadata.Property.GetMethod.IsVirtual || metadata.Property.GetMethod.IsFinal) &&
            metadata.Property.SetMethod is not null && (!metadata.Property.SetMethod.IsAbstract &&
                !metadata.Property.SetMethod.IsVirtual || metadata.Property.SetMethod.IsFinal))
        {
            throw new InvalidOperationException(
                $"{metadata.Property.DeclaringType.Name}.{metadata.Property.Name} is non-virtual " +
                $"and cannot be converted to {metadata.Unit.Name}");
        }
    }

    public void HoistMetadata<TSuperObject, TDerivedObject>(CultureInfo? culture = null)
    {
        foreach (var metadata in ((IMetadataProvider<TMetadataAttribute, TMetadata>)this).GetMetadata(
            typeof(TDerivedObject),
            culture))
        {
            if (metadata.Property.TryGetInterfaceProperty(typeof(TSuperObject), out var mappedProperty) ||
                metadata.Property.TryGetVirtualProperty(typeof(TSuperObject), out mappedProperty))
            {
                var hoistedMetadata = metadata.Clone(overrideProperty: mappedProperty, overrideCulture: culture);
                _dynamicMetadata.AddOrUpdate(mappedProperty, _ => hoistedMetadata, (_, _) => hoistedMetadata);
            }
        }
    }
}
