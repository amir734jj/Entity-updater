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

                        var existingAssignmentProfile = _profiles.FirstOrDefault(x => x != profile && x.Type == propertyInfo.PropertyType);

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
            };
        }
    }
}