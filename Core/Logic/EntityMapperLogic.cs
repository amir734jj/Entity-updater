using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EntityUpdater.Interfaces;

namespace EntityUpdater.Logic
{
    public class EntityMapperLogic : TopologicalSortLogic
    {
        public EntityMapperLogic(IReadOnlyCollection<IEntityProfile> profiles) : base(profiles)
        {
            _updateTable = new Dictionary<IEntityProfile, Action<object, object>>();
        }

        private readonly IDictionary<IEntityProfile, Action<object, object>> _updateTable;

        public Action<object, object> ResolveUpdate(IEntityProfile profile)
        {
            return BuildUpdate(profile, request => _updateTable[request]);
        }
        
        /// <summary>
        ///     Recursive function to build update for a profile
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="lazyUpdater"></param>
        /// <returns></returns>
        public Action<object, object> BuildUpdate(IEntityProfile profile,
            Func<IEntityProfile, Action<object, object>> lazyUpdater)
        {
            // This is to avoid infinite recursion
            if (_updateTable.ContainsKey(profile))
            {
                return _updateTable[profile];
            }

            return (o1, o2) =>
            {
                var entityExpr = Expression.Parameter(profile.Type);
                var dtoExpr = Expression.Parameter(profile.Type);

                var assignments = profile.Members
                    .Distinct()
                    .Select(propertyInfo =>
                    {
                        var memberAccessExprEntity = Expression.MakeMemberAccess(entityExpr, propertyInfo);
                        var memberAccessExprDto = Expression.MakeMemberAccess(dtoExpr, propertyInfo);

                        var setterMethodInfo = propertyInfo.GetSetMethod();

                        /*
                        var updateFuncExpr = Expression.Call(
                            genericUpdaterWithoutComparer,
                            memberAccessExprEntity,
                            memberAccessExprDto
                        );
                        */

                        // Type-cast the result
                        var castResultExpression = Expression.Convert(memberAccessExprDto, propertyInfo.PropertyType);

                        // Call setter of entity
                        var assignmentExpr = Expression.Call(entityExpr, setterMethodInfo, castResultExpression);

                        // Return the assignment
                        return assignmentExpr;
                    });

                // Build lambda expression
                var lambda =
                    Expression.Lambda<Action<object, object>>(Expression.Block(assignments), entityExpr, dtoExpr);

                // Store the function just compiled
                _updateTable[profile] = lambda.Compile();

                // Run the build action
                _updateTable[profile](o1, o2);
            };
        }
    }
}