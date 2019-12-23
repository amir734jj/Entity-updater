using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EntityUpdater.Interfaces;
using LaYumba.Functional.Option;
using static EntityUpdater.Logic.TopologicalSortLogic;
using static EntityUpdater.Logic.AssignmentLogic;
using static LaYumba.Functional.F;

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

            var entityExpr = Expression.Parameter(profile.Type);
            var dtoExpr = Expression.Parameter(profile.Type);

            return (o1, o2) =>
            {
                Dictionary<Type, IEntityProfile> m = null;

                var assignments = profile.Members
                    .Distinct()
                    .Select(propertyInfo => BuildAssignment(
                            propertyInfo,
                            _profilesLookup.ContainsKey,
                            x => _profilesLookup.GetValueOrDefault(x, null),
                            lazyUpdater,
                            entityExpr,
                            dtoExpr
                        )
                    );

                // Build lambda expression
                var lambda = profile.TypeSafeUpdate(Expression.Block(assignments), entityExpr, dtoExpr);

                // Store the function just compiled
                _updateTable[profile] = lambda;

                // Run the build action
                _updateTable[profile](o1, o2);
            };
        }
    }
}