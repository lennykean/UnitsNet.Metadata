using System;
using System.Linq;

using UnitsNet.Metadata.Reflection;
using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata
{
    public static class UnitExtensions
    {
        public static bool TryGetUnitInfo(this Enum unit, Type? quantityType, out UnitInfo? unitInfo)
        {
            // Check cache
            if (SimpleCache<Enum, UnitInfo>.Instance.TryGet(unit, out unitInfo))
            {
                return true;
            }

            // Check for a built-in unit type
            unitInfo = (
                from q in Quantity.Infos
                from u in q.UnitInfos
                where u.Value.Equals(unit)
                select u).SingleOrDefault();
            if (unitInfo is not null)
            {
                SimpleCache<Enum, UnitInfo>.Instance.TryAdd(unit, unitInfo);
                return true;
            }

            // Check if quantityType can be used to get a matching quantityInfo and unitInfo.
            if (unit.TryGetQuantityInfo(quantityType, out var quantityInfo))
                unitInfo = quantityInfo?.UnitInfos.SingleOrDefault(i => i.Value.Equals(unit));
            if (unitInfo is not null)
            {
                SimpleCache<Enum, UnitInfo>.Instance.TryAdd(unit, unitInfo);
                return true;
            }

            return false;
        }

        public static bool TryGetQuantityInfo(this Enum unit, Type? quantityType, out QuantityInfo? quantityInfo)
        {
            // Check cache
            if (SimpleCache<Enum, QuantityInfo>.Instance.TryGet(unit, out quantityInfo))
            {
                return true;
            }

            // Check for a built-in quantity type
            quantityInfo = (
                from q in Quantity.Infos
                where q.UnitInfos.Any(u => u.Value.Equals(unit))
                select q).SingleOrDefault();
            if (quantityInfo is not null)
            {
                SimpleCache<Enum, QuantityInfo>.Instance.TryAdd(unit, quantityInfo);
                return true;
            }

            // Check for a static QuantityInfo property on quantityType and try to invoke the getter
            if (quantityType is not null && ReflectionUtils.TryGetStaticPropertyByType(quantityType, out quantityInfo) && quantityInfo!.UnitType == unit.GetType())
            {
                SimpleCache<Enum, QuantityInfo>.Instance.TryAdd(unit, quantityInfo);
                return true;
            }

            // Check for a default public constructor, try to construct an instance of quantityType, then use the QuantityInfo instance property
            if (quantityType is not null && ReflectionUtils.TryConstructQuantity(quantityType, out var instance) && instance!.QuantityInfo.UnitType == unit.GetType())
            {
                SimpleCache<Enum, QuantityInfo>.Instance.TryAdd(unit, quantityInfo!);
                return true;
            }

            return false;
        }
    }
}