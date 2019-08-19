using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using EntityUpdater.Interfaces;
using EntityUpdater.Utility;

namespace EntityUpdater.Abstracts
{
    public abstract class EntityProfile<T> : IEntityProfile
    {
        private ImmutableList<Expression<Func<T, object>>> _memberExprs;

        private Action<T, T> _resolveAssignmentAction;

        protected EntityProfile()
        {
            _memberExprs = ImmutableList<Expression<Func<T, object>>>.Empty;
        }

        /// <summary>
        /// Update method name
        /// </summary>
        public virtual string UpdatePropertyMethodName { get; } = nameof(UpdateProperty);

        public void ResolveAssignment(IEnumerable<IEntityProfile> profiles, object entity, object dto)
        {
            if (_resolveAssignmentAction == null)
            {
                _resolveAssignmentAction = MemberExpressionUtility.GenerateAssignment(profiles, this, _memberExprs);
            }

            _resolveAssignmentAction((T) entity, (T) dto);
        }

        /// <summary>
        /// Type check if object is instance of type T
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool TypeCheck(object instance)
        {
            switch (instance)
            {
                case T _:
                    return true;
                default:
                    return false;
            }
        }

        public Type Type { get; } = typeof(T);

        /// <summary>
        /// Map property
        /// </summary>
        /// <param name="exprs"></param>
        protected MapperHelper<T> Map(params Expression<Func<T, object>>[] exprs)
        {
            _memberExprs = _memberExprs.AddRange(exprs);

            return new MapperHelper<T>(Map);
        }

        public virtual object UpdateProperty<TPropertyValue>(TPropertyValue entityPropVal, TPropertyValue dtoPropVal)
        {
            switch (dtoPropVal)
            {
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
    }

    public class MapperHelper<T>
    {
        private readonly Func<Expression<Func<T, object>>[], MapperHelper<T>> _callback;

        public MapperHelper(Func<Expression<Func<T, object>>[], MapperHelper<T>> callback)
        {
            _callback = callback;
        }

        public MapperHelper<T> Then(params Expression<Func<T, object>>[] exprs)
        {
            return _callback(exprs);
        }
    }
}