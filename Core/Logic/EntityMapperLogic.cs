using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EntityUpdater.Interfaces;
using EntityUpdater.Utility;
using static EntityUpdater.Logic.TopologicalSortLogic;

namespace EntityUpdater.Logic
{
    public class EntityMapperLogic
    {
        private readonly IDictionary<IEntityProfile, Action<object, object>> _updateTable;

        private readonly List<IEntityProfile> _profiles;

        private readonly Dictionary<Type, IEntityProfile> _profilesLookup;

        public EntityMapperLogic(IReadOnlyCollection<IEntityProfile> profiles)
        {
            _updateTable = new Dictionary<IEntityProfile, Action<object, object>>();

            _profilesLookup = profiles.ToDictionary(x => x.Type, x => x);

            _profiles = BuildGraph(profiles)().ToList();
        }

        public Action<object, object> ResolveUpdate(IEntityProfile profile)
        {
            return BuildUpdate(profile);
        }

        /// <summary>
        ///     Recursive function to build update for a profile
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public Action<object, object> BuildUpdate(IEntityProfile profile)
        {
            // This is to avoid infinite recursion
            if (_updateTable.ContainsKey(profile))
            {
                return _updateTable[profile];
            }

            var entityExpr = Expression.Parameter(profile.Type);
            var dtoExpr = Expression.Parameter(profile.Type);

            return (o1, o2) =>
            {
                var assignments = profile.Members
                    .Distinct()
                    .Select(propertyInfo => BuildAssignment(
                        profile,
                        propertyInfo,
                        entityExpr,
                        dtoExpr)
                    );

                // Build lambda expression
                var lambda = profile.TypeSafeUpdate(Expression.Block(assignments), entityExpr, dtoExpr);

                // Store the function just compiled
                _updateTable[profile] = lambda;

                // Run the build action
                _updateTable[profile](o1, o2);
            };
        }

        public Expression BuildAssignment(
            IEntityProfile currentProfile,
            PropertyInfo propertyInfo,
            ParameterExpression entityExpr,
            ParameterExpression dtoExpr
        )
        {
            var memberAccessExprEntity = Expression.MakeMemberAccess(entityExpr, propertyInfo);
            Expression memberAccessExprDto = Expression.MakeMemberAccess(dtoExpr, propertyInfo);
            var currentExpression = memberAccessExprDto;

            var setterMethodInfo = propertyInfo.GetSetMethod();

            // There is an exisiting mapper profile ...
            if (_profilesLookup.ContainsKey(propertyInfo.PropertyType) &&
                _profilesLookup[propertyInfo.PropertyType] != currentProfile)
            {
                var actionFunction = BuildUpdate(currentProfile);

                var returnTarget = Expression.Label();

                currentExpression = Expression.Block(
                    Expression.Invoke(
                        Expression.Constant(actionFunction),
                        memberAccessExprEntity,
                        memberAccessExprDto
                    ),
                    Expression.Goto(returnTarget),
                    Expression.Label(returnTarget)
                );
            }
            else if (propertyInfo.PropertyType.IsGenericType && MemberUpdaterUtility.SpecialTypes.Contains(propertyInfo.PropertyType.GetGenericTypeDefinition()))
            {
                
            }

            // Type-cast the result
            var castResultExpression = Expression.Convert(currentExpression, propertyInfo.PropertyType);

            // Call setter of entity
            var assignmentExpr = Expression.Call(entityExpr, setterMethodInfo, castResultExpression);

            // Return the assignment
            return assignmentExpr;
        }
    }
}