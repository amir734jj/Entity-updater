using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EntityUpdater.Extensions;
using EntityUpdater.Interfaces;

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
        /// <param name="profiles"></param>
        /// <param name="profile"></param>
        /// <param name="exprs"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Action<T, T> GenerateAssignment<T>(IEnumerable<IAssignmentProfile> profiles,
            IAssignmentProfile profile,
            IEnumerable<Expression<Func<T, object>>> exprs)
        {
            var type = typeof(T);
            var entityExpr = Expression.Parameter(type);
            var dtoExpr = Expression.Parameter(type);

            var assignments = exprs
                .Select(x => (Key: profile, Value: x))
                .DistinctBy(x => x.Value)
                .Select(x => (x.Key, new MemberExpressionVisitor(x.Value).ResolveMemberInfo()))
                .Select(tuple =>
                {
                    var (key, value) = tuple;

                    var propertyInfo = (PropertyInfo) value;
                    var memberAccessExprEntity = Expression.MakeMemberAccess(entityExpr, value);
                    var memberAccessExprDto = Expression.MakeMemberAccess(dtoExpr, value);

                    var existingAssignmentProfile =
                        profiles.FirstOrDefault(x => x != profile && x.Type == propertyInfo.PropertyType);

                    if (existingAssignmentProfile != null)
                    {
                        var updateFuncExpr = Expression.Call(
                            Expression.Constant(existingAssignmentProfile),
                            key.UpdatePropertyMethodName,
                            new[] {propertyInfo.PropertyType},
                            memberAccessExprEntity,
                            memberAccessExprDto
                        );

                        return updateFuncExpr;
                    }
                    else
                    {
                        var setterMethodInfo = propertyInfo.GetSetMethod();

                        if (setterMethodInfo == null)
                        {
                            throw new Exception($"Setter for member: {value.Name} does not exist");
                        }

                        var updateFuncExpr = Expression.Call(
                            Expression.Constant(key),
                            key.UpdatePropertyMethodName,
                            new[] {propertyInfo.PropertyType},
                            memberAccessExprEntity,
                            memberAccessExprDto
                        );

                        var castRsltExpr = Expression.Convert(updateFuncExpr, propertyInfo.PropertyType);

                        var assignmentExpr = Expression.Call(entityExpr, setterMethodInfo, castRsltExpr);

                        return assignmentExpr;
                    }
                });

            var body = Expression.Block(assignments);

            var lambda = Expression.Lambda<Action<T, T>>(body, entityExpr, dtoExpr);

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