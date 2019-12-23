using System;
using System.Linq.Expressions;
using System.Reflection;
using EntityUpdater.Interfaces;

namespace EntityUpdater.Logic
{
    public static class AssignmentLogic
    {
        public static Expression BuildAssignment(Expression entityExpr, Expression dtoExpr, PropertyInfo propertyInfo,
            Func<Type, bool> anyProfile, 
            Func<IEntityProfile, Action<object, object>> lazyUpdater)
        {
            var memberAccessExprEntity = Expression.MakeMemberAccess(entityExpr, propertyInfo);
            var memberAccessExprDto = Expression.MakeMemberAccess(dtoExpr, propertyInfo);

            var setterMethodInfo = propertyInfo.GetSetMethod();

            // There is an exisiting mapper profile ...
            if (anyProfile(propertyInfo.PropertyType))
            {
                
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