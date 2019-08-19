using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
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

        public MethodInfo ComparerMethodInfo { get; private set; }

        /// <summary>
        /// Map property
        /// </summary>
        /// <param name="exprs"></param>
        protected MapperHelper<T> Map(params Expression<Func<T, object>>[] exprs)
        {
            _memberExprs = _memberExprs.AddRange(exprs);

            return new MapperHelper<T>(Map, comparerMethodInfo => ComparerMethodInfo = comparerMethodInfo);
        }
    }

    public class MapperHelper<T>
    {
        private readonly Func<Expression<Func<T, object>>[], MapperHelper<T>> _propertyDef;

        private readonly Action<MethodInfo> _comparisonDef;

        public MapperHelper(Func<Expression<Func<T, object>>[], MapperHelper<T>> propertyDef,
            Action<MethodInfo> comparisonDef)
        {
            _propertyDef = propertyDef;
            _comparisonDef = comparisonDef;
        }

        public MapperHelper<T> Then(params Expression<Func<T, object>>[] exprs)
        {
            return _propertyDef(exprs);
        }

        public void Compare(Func<T, T, bool> comparison)
        {
            _comparisonDef(comparison.Method);
        }
    }
}