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
        /// <param name="expressions"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Action<T, T> GenerateAssignment<T>(IEnumerable<IEntityProfile> profiles,
            IEntityProfile profile,
            IEnumerable<Expression<Func<T, object>>> expressions)
        {
            var entityExpr = Expression.Parameter(profile.Type);
            var dtoExpr = Expression.Parameter(profile.Type);

            var assignments = expressions
                .Distinct()
                .Select(x => new MemberExpressionLogic.MemberExpressionVisitor(x).ResolveMemberInfo())
                .Select(memberInfo =>
                {
                    var propertyInfo = (PropertyInfo) memberInfo;
                    var memberAccessExprEntity = Expression.MakeMemberAccess(entityExpr, memberInfo);
                    var memberAccessExprDto = Expression.MakeMemberAccess(dtoExpr, memberInfo);

                    var existingAssignmentProfile =
                        profiles.FirstOrDefault(x => x != profile && x.Type == propertyInfo.PropertyType);

                    if (existingAssignmentProfile != null)
                    {
                        var genericUpdaterWithComparer =
                            UpdatePropertyWithComparerMethodInfo.MakeGenericMethod(propertyInfo.PropertyType);
                        var profileComparerExpr = Expression.Constant(existingAssignmentProfile.ComparerMethodInfo);

                        var updateFuncExpr = Expression.Call(
                            genericUpdaterWithComparer,
                            memberAccessExprEntity,
                            memberAccessExprDto,
                            profileComparerExpr);

                        return (Expression) updateFuncExpr;
                    }
                    else
                    {
                        var genericUpdaterWithoutComparer =
                            UpdatePropertyWithoutComparerMethodInfo.MakeGenericMethod(propertyInfo.PropertyType);
                        var setterMethodInfo = propertyInfo.GetSetMethod();

                        if (setterMethodInfo == null)
                        {
                            throw new Exception($"Setter for member: {memberInfo.Name} does not exist");
                        }

                        var updateFuncExpr = Expression.Call(
                            genericUpdaterWithoutComparer,
                            memberAccessExprEntity,
                            memberAccessExprDto
                        );

                        var castResultExpression = Expression.Convert(updateFuncExpr, propertyInfo.PropertyType);

                        var assignmentExpr = Expression.Call(entityExpr, setterMethodInfo, castResultExpression);

                        return assignmentExpr;
                    }
                });

            var body = Expression.Block(assignments);

            var lambda = Expression.Lambda<Action<T, T>>(body, entityExpr, dtoExpr);

            return lambda.Compile();
        }
    }
}