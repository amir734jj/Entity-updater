using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityUpdater.Extensions
{
    public static class MemberExpressionLogic
    {
        public static PropertyInfo ResolvePropertyInfo<TSource, TProperty>(
            this Expression<Func<TSource, TProperty>> memberExpression)
        {
            return (PropertyInfo) new MemberExpressionVisitor(memberExpression).ResolveMemberInfo();
        }

        /// <inheritdoc />
        /// <summary>
        /// Expression visitor to resolve MemberInfo from an Expression
        /// </summary>
        public sealed class MemberExpressionVisitor : ExpressionVisitor
        {
            private ImmutableList<MemberInfo> _members;

            private readonly Expression _expr;

            /// <inheritdoc />
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
            public MemberInfo ResolveMemberInfo() => _members.Count switch
            {
                1 => _members.First(),
                _ => throw new Exception($"Expression: `{_expr}` is not a valid member expression")
            };

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
}