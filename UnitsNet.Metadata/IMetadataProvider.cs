using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.Reflection;
using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata;

/// <summary>
/// A source of quantity metadata for the properties of a type.
/// </summary>
/// <typeparam name="TMetadataAttribue">The metadata attribute type</typeparam>
/// <typeparam name="TMetadata">The metadata type</typeparam>
public interface IMetadataProvider<TMetadataAttribue, TMetadata>
    where TMetadataAttribue : QuantityAttribute, IMetadataAttribute<TMetadataAttribue, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    /// <summary>
    /// Tries to get the metadata of a property.
    /// </summary>
    /// <param name="property">The property to get the metadata of</param>
    /// <param name="metadata">The metadata of the property, or <see langword="null"/> when it has none</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    bool TryGetMetadata(
        PropertyInfo property,
        [NotNullWhen(true)] out TMetadata? metadata,
        CultureInfo? culture = null);

    /// <summary>
    /// Validates the metadata of a property.
    /// </summary>
    /// <param name="property">The property to validate</param>
    void ValidateMetadata(PropertyInfo property);

    /// <summary>
    /// Gets the metadata of the properties of a type.
    /// </summary>
    /// <param name="type">The type whose properties to describe</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public virtual IEnumerable<TMetadata> GetMetadata(Type type, CultureInfo? culture = null)
    {
        var cache = EphemeralValueCache<
            (IMetadataProvider<TMetadataAttribue, TMetadata>, Type type, CultureInfo culture, bool useDefaultCulture),
            IEnumerable<TMetadata>>.GlobalInstance;
        return cache.GetOrAdd((this, type, culture ?? CultureInfo.CurrentCulture, culture is null), key =>
        {
            IEnumerable<TMetadata> Get(Type type) => (
                from property in type.IsInterface
                    ? type.GetInterfaces().Append(type).SelectMany(i => i.GetProperties())
                    : type.GetProperties()
                let m = (hasMetadata: TryGetMetadata(property, out var metadata, culture), metadata)
                where m.hasMetadata
                select m.metadata)
                .ToList();

            var metadata = Get(key.type);

            if (!metadata.Any())
            {
                var elementType = key.type switch
                {
                    { IsArray: true } => key.type.GetElementType(),
                    { IsInterface: true } when key.type.GetGenericTypeDefinition() == typeof(IEnumerable<>) => key.type
                        .GenericTypeArguments[0],
                    { IsInterface: true } => (
                        from interfaceType in key.type.GetInterfaces()
                        where interfaceType.IsGenericType &&
                            interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                        select interfaceType.GenericTypeArguments[0])
                        .SingleOrDefault(),
                    _ => (
                        from interfaceType in key.type.GetInterfaces()
                        where interfaceType.IsGenericType &&
                            interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                        let interfaceMap = key.type.GetInterfaceMap(interfaceType)
                        from interfaceMethod in interfaceMap.InterfaceMethods.Select((method, index) => (method, index))
                        where interfaceMethod.method.Name == "GetEnumerator" &&
                            !interfaceMethod.method.GetParameters().Any()
                        select interfaceMap.TargetMethods[interfaceMethod.index].DeclaringType.GenericTypeArguments[0])
                        .SingleOrDefault()
                };
                if (elementType != null)
                {
                    metadata = Get(elementType);
                }
            }

            return metadata;
        });
    }

    /// <summary>
    /// Validates the metadata of all properties of a type.
    /// </summary>
    /// <param name="type">The type whose properties to validate</param>
    public virtual void ValidateAllMetadata(Type type)
    {
        foreach (var metadata in GetMetadata(type))
        {
            ValidateMetadata(metadata.Property);
        }
    }

    /// <summary>
    /// Gets the metadata of the properties of an object, keyed by property name.
    /// </summary>
    /// <typeparam name="TObject">The type of the object</typeparam>
    /// <param name="obj">The object to describe</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public IReadOnlyDictionary<string, TMetadata> GetObjectMetadata<TObject>(TObject obj, CultureInfo? culture = null)
        where TObject : class
    {
        ValidateAllMetadata(typeof(TObject));

        return new MetadataDictionary<TMetadata>(GetMetadata(typeof(TObject), culture));
    }

    /// <summary>
    /// Gets the value of a property as a quantity.
    /// </summary>
    /// <typeparam name="TObject">The type of the object</typeparam>
    /// <param name="obj">The object that holds the property</param>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public IQuantity GetQuantity<TObject>(TObject obj, string propertyName, CultureInfo? culture = null)
        where TObject : class
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var property = typeof(TObject).GetPropertyFlat(propertyName) ??
            throw new InvalidOperationException($"{propertyName} is not a property of {typeof(TObject).Name}");

        return GetQuantity(obj, property, culture);
    }

    /// <summary>
    /// Gets the value of a property as a quantity.
    /// </summary>
    /// <typeparam name="TObject">The type of the object</typeparam>
    /// <param name="obj">The object that holds the property</param>
    /// <param name="propertySelectorExpression">The expression selecting the property</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public IQuantity GetQuantity<TObject>(
        TObject obj,
        Expression<Func<TObject, QuantityValue>> propertySelectorExpression,
        CultureInfo? culture = null)
        where TObject : class
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var propertyName = propertySelectorExpression.ExtractPropertyName();
        var property = typeof(TObject).GetPropertyFlat(propertyName)!;

        return GetQuantity(obj, property, culture);
    }

    /// <summary>
    /// Converts the value of a property to another unit.
    /// </summary>
    /// <typeparam name="TObject">The type of the object</typeparam>
    /// <param name="obj">The object that holds the property</param>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="to">The unit to convert to</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public IQuantity ConvertQuantity<TObject>(TObject obj, string propertyName, Enum to, CultureInfo? culture = null)
        where TObject : class
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var property = typeof(TObject).GetPropertyFlat(propertyName) ??
            throw new InvalidOperationException($"{propertyName} is not a property of {typeof(TObject).Name}");

        return ConvertQuantity(obj, property, to, culture);
    }

    /// <summary>
    /// Converts the value of a property to another unit.
    /// </summary>
    /// <typeparam name="TObject">The type of the object</typeparam>
    /// <param name="obj">The object that holds the property</param>
    /// <param name="propertySelectorExpression">The expression selecting the property</param>
    /// <param name="to">The unit to convert to</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public IQuantity ConvertQuantity<TObject>(
        TObject obj,
        Expression<Func<TObject, QuantityValue>> propertySelectorExpression,
        Enum to,
        CultureInfo? culture = null)
        where TObject : class
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var propertyName = propertySelectorExpression.ExtractPropertyName();
        var property = typeof(TObject).GetPropertyFlat(propertyName)!;

        return ConvertQuantity(obj, property, to, culture);
    }

    /// <summary>
    /// Sets the value of a property from a quantity.
    /// </summary>
    /// <typeparam name="TObject">The type of the object</typeparam>
    /// <param name="obj">The object that holds the property</param>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="quantity">The quantity to set</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public IQuantity SetQuantity<TObject>(
        TObject obj,
        string propertyName,
        IQuantity quantity,
        CultureInfo? culture = null)
        where TObject : class
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var property = typeof(TObject).GetPropertyFlat(propertyName) ??
            throw new InvalidOperationException($"{propertyName} is not a property of {typeof(TObject).Name}");

        return SetQuantity(obj, property, quantity, culture);
    }

    /// <summary>
    /// Sets the value of a property from a quantity.
    /// </summary>
    /// <typeparam name="TObject">The type of the object</typeparam>
    /// <param name="obj">The object that holds the property</param>
    /// <param name="propertySelectorExpression">The expression selecting the property</param>
    /// <param name="quantity">The quantity to set</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public IQuantity SetQuantity<TObject>(
        TObject obj,
        Expression<Func<TObject, QuantityValue>> propertySelectorExpression,
        IQuantity quantity,
        CultureInfo? culture = null)
        where TObject : class
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var propertyName = propertySelectorExpression.ExtractPropertyName();
        var property = typeof(TObject).GetPropertyFlat(propertyName)!;

        return SetQuantity(obj, property, quantity, culture);
    }

    private IQuantity GetQuantity<TObject>(TObject obj, PropertyInfo property, CultureInfo? culture = null)
        where TObject : class
    {
        if (TryGetMetadata(property, out var metadata, culture) is not true || metadata.Unit is null)
        {
            throw new InvalidOperationException(
                $"Unit metadata does not exist for {property.DeclaringType.Name}.{property.Name}.");
        }

        metadata.Validate();

        var value = obj.GetQuantityValueFromProperty(property);

        var unitMetadata = metadata.Unit!;
        var quantityTypeMetadata = unitMetadata.QuantityType;

        return value.AsQuantity(unitMetadata.UnitInfo.Value, quantityTypeMetadata.QuantityInfo.QuantityType);
    }

    private IQuantity ConvertQuantity<TObject>(TObject obj, PropertyInfo property, Enum to, CultureInfo? culture = null)
        where TObject : class
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var (propertyMetadata, conversionMetadata) = property.GetConversionMetadatas(to, this, culture);
        var value = obj.GetQuantityValueFromProperty(property);

        if (!propertyMetadata.TryConvertQuantity(value, conversionMetadata, out var quantity))
        {
            throw new InvalidOperationException(
                $"Invalid conversion from {propertyMetadata.QuantityType.Name} " +
                $"to {conversionMetadata.QuantityType.Name}.");
        }

        return quantity;
    }

    private IQuantity SetQuantity<TObject>(TObject obj, PropertyInfo property, IQuantity quantity, CultureInfo? culture)
        where TObject : class
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        if (TryGetMetadata(property, out var metadata, culture) is not true || metadata.Unit is null)
        {
            throw new InvalidOperationException(
                $"Unit metadata does not exist for {property.DeclaringType.Name}.{property.Name}.");
        }

        metadata.Validate();

        var (propertyMetadata, conversionMetadata) = property.GetConversionMetadatas(quantity.Unit, this, culture);
        if (!conversionMetadata.TryConvertQuantity(quantity.Value, propertyMetadata, out var convertedQuantity))
        {
            throw new InvalidOperationException(
                $"Invalid conversion from {conversionMetadata.QuantityType.Name} " +
                $"to {propertyMetadata.QuantityType.Name}.");
        }

        property.SetMethod.Invoke(obj, new[] { convertedQuantity.Value.ToNumericValue(property.PropertyType) });

        return convertedQuantity;
    }
}
