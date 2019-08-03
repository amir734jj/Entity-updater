using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityUpdater.Utility
{
    /// <summary>
    /// Utility to create an assignment 
    /// </summary>
    public static class MemberExpressionUtility
    {
        /// <summary>
        /// Generate assignment function from the provided member
        /// </summary>
        /// <param name="exprs"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Action<T, T> GenerateAssignment<T>(IEnumerable<Expression<Func<T, object>>> exprs)
        {
            var type = typeof(T);
            var entity = Expression.Parameter(type);
            var dto = Expression.Parameter(type);

            var assignments = exprs.Select(expr =>
            {
                var memberInfo = new MemberExpressionVisitor(expr).ResolveMemberInfo();
                var memberAccessExpr = Expression.MakeMemberAccess(dto, memberInfo);
                var setterMethodInfo = ((PropertyInfo) memberInfo).GetSetMethod();

                if (setterMethodInfo == null)
                {
                    throw new Exception($"Setter for member: {memberInfo.Name} does not exist");
                }

                var assignmentExpr = Expression.Call(entity, setterMethodInfo, memberAccessExpr);

                return assignmentExpr;
            });

            var body = Expression.Block(assignments);

            var lambda = Expression.Lambda<Action<T, T>>(body, entity, dto);

            return lambda.Compile();
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Expression visitor to resolve MemberInfo from an Expression
    /// </summary>
    public sealed class MemberExpressionVisitor : ExpressionVisitor
    {
        private ImmutableList<MemberInfo> _members;

        private readonly Expression _expr;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="expr"></param>
        public MemberExpressionVisitor(Expression expr)
        {
            _expr = expr;
            _members = ImmutableList.Create<MemberInfo>();

            Visit(expr);
        }

        /// <summary>
        /// Return the MemberInfo, throw an exception if count of MemberInfos is not equal to one
        /// </summary>
        /// <returns></returns>
        public MemberInfo ResolveMemberInfo()
        {
            return _members.Count == 1
                ? _members.First()
                : throw new Exception($"Expression: `{_expr}` is not a valid member expression");
        }

        /// <inheritdoc />
        /// <summary>
        /// Add the MemberInfo to the list (there should be a single MemberInfo at the end)
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            _members = _members.Add(node.Member);

            return base.VisitMember(node);
        }
    }
}