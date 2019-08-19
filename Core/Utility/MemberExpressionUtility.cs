using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EntityUpdater.Extensions;
using EntityUpdater.Interfaces;
using static EntityUpdater.Utility.MemberUpdaterUtility;

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
        public static Action<T, T> GenerateAssignment<T>(IEnumerable<IEntityProfile> profiles,
            IEntityProfile profile,
            IEnumerable<Expression<Func<T, object>>> exprs)
        {
            var entityExpr = Expression.Parameter(profile.Type);
            var dtoExpr = Expression.Parameter(profile.Type);

            var assignments = exprs
                .DistinctBy(x => x)
                .Select(x => new MemberExpressionVisitor(x).ResolveMemberInfo())
                .Select(memberInfo =>
                {
                    var propertyInfo = (PropertyInfo) memberInfo;
                    var memberAccessExprEntity = Expression.MakeMemberAccess(entityExpr, memberInfo);
                    var memberAccessExprDto = Expression.MakeMemberAccess(dtoExpr, memberInfo);

                    var existingAssignmentProfile =
                        profiles.FirstOrDefault(x => x != profile && x.Type == propertyInfo.PropertyType);

                    if (existingAssignmentProfile != null)
                    {
                        var genericUpdatorWithComparer =
                            UpdatePropertyWithComparerMethodInfo.MakeGenericMethod(propertyInfo.PropertyType);
                        var profileComparerExpr = Expression.Constant(existingAssignmentProfile.ComparerMethodInfo);

                        var updateFuncExpr = Expression.Call(
                            genericUpdatorWithComparer,
                            memberAccessExprEntity,
                            memberAccessExprDto,
                            profileComparerExpr);

                        return (Expression) updateFuncExpr;
                    }
                    else
                    {
                        var genericUpdatorWithoutComparer =
                            UpdatePropertyWithoutComparerMethodInfo.MakeGenericMethod(propertyInfo.PropertyType);
                        var setterMethodInfo = propertyInfo.GetSetMethod();

                        if (setterMethodInfo == null)
                        {
                            throw new Exception($"Setter for member: {memberInfo.Name} does not exist");
                        }

                        var updateFuncExpr = Expression.Call(
                            genericUpdatorWithoutComparer,
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
            switch (_members.Count)
            {
                case 1:
                    return _members.First();
                default:
                    throw new Exception($"Expression: `{_expr}` is not a valid member expression");
            }
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