using System;
using System.Linq.Expressions;
using System.Reflection;
using EntityUpdater.Interfaces;

namespace EntityUpdater.Logic
{
    public static class AssignmentLogic
    {
        public static Expression BuildAssignment(PropertyInfo propertyInfo,
            Func<Type, bool> anyProfile,
            Func<Type, IEntityProfile> getProfile,
            Func<IEntityProfile, Action<object, object>> lazyUpdater,
            Expression entityExpr = null,
            Expression dtoExpr = null
        )
        {
            var memberAccessExprEntity = Expression.MakeMemberAccess(entityExpr, propertyInfo);
            Expression memberAccessExprDto = Expression.MakeMemberAccess(dtoExpr, propertyInfo);

            var setterMethodInfo = propertyInfo.GetSetMethod();

            // There is an exisiting mapper profile ...
            if (anyProfile(propertyInfo.PropertyType))
            {
                var entityPropExpr = Expression.Parameter(propertyInfo.PropertyType);
                Expression dtoPropExpr = Expression.Parameter(propertyInfo.PropertyType);

                var profile = getProfile(propertyInfo.PropertyType);

                memberAccessExprDto = BuildAssignment(propertyInfo, anyProfile, getProfile, lazyUpdater, entityPropExpr, entityPropExpr);
            }
            else
            {
            }


            // Type-cast the result
            var castResultExpression = Expression.Convert(memberAccessExprDto, propertyInfo.PropertyType);

            // Call setter of entity
            var assignmentExpr = Expression.Call(entityExpr, setterMethodInfo, castResultExpression);

            // Return the assignment
            return assignmentExpr;
        }
    }
}