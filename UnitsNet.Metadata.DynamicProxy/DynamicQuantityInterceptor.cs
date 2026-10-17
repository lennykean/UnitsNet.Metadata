using System.Collections.Concurrent;
using System.Reflection;

using Castle.DynamicProxy;

using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata.DynamicProxy;

internal sealed class DynamicQuantityInterceptor<TObject, TMetadataAttribute, TMetadata> : IInterceptor
    where TObject : class
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    private readonly ConcurrentDictionary<MethodInfo, PropertyInfo?> _methodPropertyMap = new();

    public DynamicQuantityInterceptor(DynamicProxyMetadataProvider<TMetadataAttribute, TMetadata> metadataProvider)
    {
        MetadataProvider = metadataProvider;
    }

    public DynamicProxyMetadataProvider<TMetadataAttribute, TMetadata> MetadataProvider { get; }

    public void Intercept(IInvocation invocation)
    {
        invocation.Proceed();

        if (typeof(IMetadataProvider<TMetadataAttribute, TMetadata>).IsAssignableFrom(invocation.TargetType))
        {
            return;
        }

        var targetProperty = _methodPropertyMap.GetOrAdd(
            invocation.GetConcreteMethodInvocationTarget(),
            targetMethod => targetMethod.ReflectedType.GetProperties()
                .SingleOrDefault(p => p.GetMethod == targetMethod || p.SetMethod == targetMethod));
        if (targetProperty is null || !MetadataProvider.TryGetBaseMetadata(targetProperty, out var targetMetadata) ||
            targetMetadata.Unit is null)
        {
            return;
        }

        var property = _methodPropertyMap.GetOrAdd(
            invocation.Method,
            method => method.ReflectedType.GetProperties()
                .SingleOrDefault(p => p.GetMethod == method || p.SetMethod == method));
        if (property is null || !MetadataProvider.TryGetMetadata(property, out var metadata) || metadata.Unit is null)
        {
            return;
        }

        if (targetMetadata.Unit.UnitInfo.Value == metadata.Unit.UnitInfo.Value)
        {
            return;
        }

        if (invocation.Arguments.Any())
        {
            var quantity = ConvertQuantity(
                from: metadata.Unit,
                to: targetMetadata.Unit,
                value: invocation.Arguments[0]);
            targetProperty.SetValue(
                invocation.InvocationTarget,
                Convert.ChangeType(quantity.Value, property.PropertyType));
        }
        else
        {
            var quantity = ConvertQuantity(from: targetMetadata.Unit, to: metadata.Unit, value: invocation.ReturnValue);
            invocation.ReturnValue = Convert.ChangeType(quantity.Value, property.PropertyType);
        }
    }

    private static IQuantity ConvertQuantity(UnitMetadata from, UnitMetadata to, object value)
    {
        if (!from.TryConvertQuantity(Convert.ToDouble(value), to, out var quantity))
        {
            throw new InvalidOperationException(
                $"Invalid unit conversion from {from.UnitInfo.Value} to {to.UnitInfo.Value}.");
        }

        return quantity;
    }
}
