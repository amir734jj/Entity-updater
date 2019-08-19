using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntityUpdater.Utility
{
    public class MemberUpdaterUtility
    {
        private static readonly MemberUpdaterUtility Instance = new MemberUpdaterUtility();

        public static readonly MethodInfo UpdatePropertyWithComparerMethodInfo = Instance.GetType()
            .GetMethod(nameof(UpdatePropertyWithComparer), BindingFlags.Public | BindingFlags.Static);

        public static readonly MethodInfo UpdatePropertyWithoutComparerMethodInfo = Instance.GetType()
            .GetMethod(nameof(UpdatePropertyWithoutComparer), BindingFlags.Public | BindingFlags.Static);
        
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public static TPropertyValue UpdatePropertyWithComparer<TPropertyValue>(TPropertyValue entityPropVal,
            TPropertyValue dtoPropVal,
            Func<TPropertyValue, TPropertyValue, bool> comparer)
        {
            switch (dtoPropVal)
            {
                case List<TPropertyValue> dtoPropValList when entityPropVal is List<TPropertyValue> entityPropValList:
                    // Apply addition
                    foreach (var dtoPropValListItem in dtoPropValList)
                    {
                        if (!entityPropValList.Any(x => comparer(x, dtoPropValListItem)))
                        {
                            entityPropValList.Add(dtoPropValListItem);
                        }
                    }

                    // Apply deletion
                    foreach (var entityPropValListItem in entityPropValList)
                    {
                        if (!dtoPropValList.Any(x => comparer(x, entityPropValListItem)))
                        {
                            entityPropValList.Remove(entityPropValListItem);
                        }
                    }

                    return entityPropVal;

                case IList dtoPropValList when entityPropVal is IList entityPropValList:
                    // Apply addition
                    foreach (var dtoPropValListItem in dtoPropValList)
                    {
                        if (!entityPropValList.Contains(dtoPropValListItem))
                        {
                            entityPropValList.Add(dtoPropValListItem);
                        }
                    }

                    // Apply deletion
                    foreach (var entityPropValListItem in entityPropValList)
                    {
                        if (!dtoPropValList.Contains(entityPropValListItem))
                        {
                            entityPropValList.Remove(entityPropValListItem);
                        }
                    }

                    return entityPropVal;
                case IDictionary dtoPropValDict when entityPropVal is IDictionary entityPropValDict:
                    // Apply addition
                    foreach (DictionaryEntry dtoPropValDictEntry in dtoPropValDict)
                    {
                        if (!entityPropValDict.Contains(dtoPropValDictEntry.Key))
                        {
                            entityPropValDict[dtoPropValDictEntry.Key] = dtoPropValDictEntry.Value;
                        }
                    }

                    // Apply deletion
                    foreach (DictionaryEntry entityPropValDictEntry in entityPropValDict)
                    {
                        if (!dtoPropValDict.Contains(entityPropValDictEntry.Key))
                        {
                            entityPropValDict.Remove(entityPropValDictEntry.Key);
                        }
                    }

                    return entityPropVal;
                case object x when x == (object) default(TPropertyValue):
                    return dtoPropVal;
                default:
                    return dtoPropVal;
            }
        }

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public static TPropertyValue UpdatePropertyWithoutComparer<TPropertyValue>(TPropertyValue entityPropVal,
            TPropertyValue dtoPropVal)
        {
            bool Comparer(TPropertyValue x, TPropertyValue y) => x == null && y == null || x != null && x.Equals(y);

            return UpdatePropertyWithComparer(entityPropVal, dtoPropVal, Comparer);
        }
    }
}