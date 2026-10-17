using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata;

/// <summary>
/// Extension methods for reading and writing the quantity properties of an object.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Gets the metadata of all quantity properties of the object.
    /// </summary>
    /// <typeparam name="TObject">The type of the object that holds the quantity properties</typeparam>
    /// <typeparam name="TMetadataAttribute">The metadata attribute type</typeparam>
    /// <typeparam name="TMetadata">The metadata type</typeparam>
    /// <param name="obj">The object that holds the quantity properties</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public static IReadOnlyDictionary<string, TMetadata> GetObjectMetadata<TObject, TMetadataAttribute, TMetadata>(
        this TObject obj,
        CultureInfo? culture = null)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        return obj.GetMetadataProvider<TObject, TMetadataAttribute, TMetadata>().GetObjectMetadata(obj, culture);
    }

    /// <summary>
    /// Gets the metadata of all quantity properties of the object.
    /// </summary>
    /// <typeparam name="TObject">The type of the object that holds the quantity properties</typeparam>
    /// <param name="obj">The object that holds the quantity properties</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public static IReadOnlyDictionary<string, QuantityMetadata> GetObjectMetadata<TObject>(
        this TObject obj,
        CultureInfo? culture = null)
        where TObject : class
    {
        return obj.GetObjectMetadata<TObject, QuantityAttribute, QuantityMetadata>(culture);
    }

    /// <summary>
    /// Gets the value of a quantity property as a quantity.
    /// </summary>
    /// <typeparam name="TObject">The type of the object that holds the quantity properties</typeparam>
    /// <param name="obj">The object that holds the quantity properties</param>
    /// <param name="propertyName">The name of the quantity property</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public static IQuantity GetQuantity<TObject>(this TObject obj, string propertyName, CultureInfo? culture = null)
        where TObject : class
    {
        return obj.GetMetadataProvider<TObject, QuantityAttribute, QuantityMetadata>()
            .GetQuantity(obj, propertyName, culture);
    }

    /// <summary>
    /// Gets the value of a quantity property as a quantity.
    /// </summary>
    /// <typeparam name="TObject">The type of the object that holds the quantity properties</typeparam>
    /// <param name="obj">The object that holds the quantity properties</param>
    /// <param name="propertySelectorExpression">The expression selecting the quantity property</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public static IQuantity GetQuantity<TObject>(
        this TObject obj,
        Expression<Func<TObject, QuantityValue>> propertySelectorExpression,
        CultureInfo? culture = null)
        where TObject : class
    {
        return obj.GetMetadataProvider<TObject, QuantityAttribute, QuantityMetadata>()
            .GetQuantity(obj, propertySelectorExpression, culture);
    }

    /// <summary>
    /// Converts the value of a quantity property to another unit.
    /// </summary>
    /// <typeparam name="TObject">The type of the object that holds the quantity properties</typeparam>
    /// <param name="obj">The object that holds the quantity properties</param>
    /// <param name="propertyName">The name of the quantity property</param>
    /// <param name="to">The unit to convert to</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public static IQuantity ConvertQuantity<TObject>(
        this TObject obj,
        string propertyName,
        Enum to,
        CultureInfo? culture = null)
        where TObject : class
    {
        return obj.GetMetadataProvider<TObject, QuantityAttribute, QuantityMetadata>()
            .ConvertQuantity(obj, propertyName, to, culture);
    }

    /// <summary>
    /// Converts the value of a quantity property to another unit.
    /// </summary>
    /// <typeparam name="TObject">The type of the object that holds the quantity properties</typeparam>
    /// <param name="obj">The object that holds the quantity properties</param>
    /// <param name="propertySelectorExpression">The expression selecting the quantity property</param>
    /// <param name="to">The unit to convert to</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public static IQuantity ConvertQuantity<TObject>(
        this TObject obj,
        Expression<Func<TObject, QuantityValue>> propertySelectorExpression,
        Enum to,
        CultureInfo? culture = null)
        where TObject : class
    {
        return obj.GetMetadataProvider<TObject, QuantityAttribute, QuantityMetadata>()
            .ConvertQuantity(obj, propertySelectorExpression, to, culture);
    }

    /// <summary>
    /// Sets the value of a quantity property from a quantity.
    /// </summary>
    /// <typeparam name="TObject">The type of the object that holds the quantity properties</typeparam>
    /// <typeparam name="TMetadataAttribute">The metadata attribute type</typeparam>
    /// <typeparam name="TMetadata">The metadata type</typeparam>
    /// <param name="obj">The object that holds the quantity properties</param>
    /// <param name="propertyName">The name of the quantity property</param>
    /// <param name="quantity">The quantity to set</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public static IQuantity SetQuantity<TObject, TMetadataAttribute, TMetadata>(
        this TObject obj,
        string propertyName,
        IQuantity quantity,
        CultureInfo? culture = null)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        return obj.GetMetadataProvider<TObject, TMetadataAttribute, TMetadata>()
            .SetQuantity(obj, propertyName, quantity, culture);
    }

    /// <summary>
    /// Sets the value of a quantity property from a quantity.
    /// </summary>
    /// <typeparam name="TObject">The type of the object that holds the quantity properties</typeparam>
    /// <typeparam name="TMetadataAttribute">The metadata attribute type</typeparam>
    /// <typeparam name="TMetadata">The metadata type</typeparam>
    /// <param name="obj">The object that holds the quantity properties</param>
    /// <param name="propertySelectorExpression">The expression selecting the quantity property</param>
    /// <param name="quantity">The quantity to set</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public static IQuantity SetQuantity<TObject, TMetadataAttribute, TMetadata>(
        this TObject obj,
        Expression<Func<TObject, QuantityValue>> propertySelectorExpression,
        IQuantity quantity,
        CultureInfo? culture = null)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        return obj.GetMetadataProvider<TObject, TMetadataAttribute, TMetadata>()
            .SetQuantity(obj, propertySelectorExpression, quantity, culture);
    }

    /// <summary>
    /// Sets the value of a quantity property from a quantity.
    /// </summary>
    /// <typeparam name="TObject">The type of the object that holds the quantity properties</typeparam>
    /// <param name="obj">The object that holds the quantity properties</param>
    /// <param name="propertyName">The name of the quantity property</param>
    /// <param name="quantity">The quantity to set</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public static IQuantity SetQuantity<TObject>(
        this TObject obj,
        string propertyName,
        IQuantity quantity,
        CultureInfo? culture = null)
        where TObject : class
    {
        return obj.GetMetadataProvider<TObject, QuantityAttribute, QuantityMetadata>()
            .SetQuantity(obj, propertyName, quantity, culture);
    }

    /// <summary>
    /// Sets the value of a quantity property from a quantity.
    /// </summary>
    /// <typeparam name="TObject">The type of the object that holds the quantity properties</typeparam>
    /// <param name="obj">The object that holds the quantity properties</param>
    /// <param name="propertySelectorExpression">The expression selecting the quantity property</param>
    /// <param name="quantity">The quantity to set</param>
    /// <param name="culture">The culture of the unit abbreviations</param>
    public static IQuantity SetQuantity<TObject>(
        this TObject obj,
        Expression<Func<TObject, QuantityValue>> propertySelectorExpression,
        IQuantity quantity,
        CultureInfo? culture = null)
        where TObject : class
    {
        return obj.GetMetadataProvider<TObject, QuantityAttribute, QuantityMetadata>()
            .SetQuantity(obj, propertySelectorExpression, quantity, culture);
    }

    private static IMetadataProvider<TMetadataAttribute, TMetadata>
        GetMetadataProvider<TObject, TMetadataAttribute, TMetadata>(this TObject obj)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        if (obj is IMetadataProvider<TMetadataAttribute, TMetadata> metadataProvider)
        {
            return metadataProvider;
        }

        return GlobalMetadataProvider<TMetadataAttribute, TMetadata>.Instance;
    }
}
