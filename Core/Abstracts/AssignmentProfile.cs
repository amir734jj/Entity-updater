using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using EntityUpdater.Interfaces;
using EntityUpdater.Utility;

namespace EntityUpdater.Abstracts
{
    public abstract class AssignmentProfile<T> : IAssignmentProfile
    {
        private ImmutableList<Expression<Func<T, object>>> _memberExprs = ImmutableList<Expression<Func<T, object>>>.Empty;

        private Action<T, T> _resolveAssignmentAction;

        public void ResolveAssignment(object entity, object dto)
        {
            if (_resolveAssignmentAction == null)
            {
                _resolveAssignmentAction = MemberExpressionUtility.GenerateAssignment(_memberExprs);
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
        
        /// <summary>
        /// Map property
        /// </summary>
        /// <param name="exprs"></param>
        protected MapperHelper<T> Map(params Expression<Func<T, object>>[] exprs)
        {
            _memberExprs = _memberExprs.AddRange(exprs);

            return new MapperHelper<T>(Map);
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