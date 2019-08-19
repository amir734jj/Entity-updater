using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using EntityUpdater.Extensions;
using EntityUpdater.Interfaces;

namespace EntityUpdater
{
    public class AssignmentUtilityHelper : IAssignmentUtilityHelper
    {
        public void Assembly(params string[] names)
        {
            var assemblies = names.Select(System.Reflection.Assembly.Load).ToArray();

            Assembly(assemblies);
        }

        public void Assembly(params Assembly[] assemblies)
        {
            var classMapType = typeof(IAssignmentProfile);

            var rslt = assemblies.SelectMany(assembly => assembly.DefinedTypes
                .Where(x => x.IsClass && !x.IsAbstract && classMapType.IsAssignableFrom(x))
                .Select(x => x.Instantiate<IAssignmentProfile>()));
            
            Profiles = Profiles.AddRange(rslt);
        }

        public void Profile<T>(T instance) where T : IAssignmentProfile
        {
            Profiles = Profiles.Add(instance);
        }

        public void Profile<T>() where T : IAssignmentProfile
        {
            Profiles = Profiles.Add(typeof(T).Instantiate<IAssignmentProfile>());
        }

        public ImmutableList<IAssignmentProfile> Profiles { get; private set; }

        protected AssignmentUtilityHelper()
        {
            Profiles = ImmutableList<IAssignmentProfile>.Empty;
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        /// <returns></returns>
        public static IAssignmentUtilityHelper New()
        {
            return new AssignmentUtilityHelper();
        }
    }

    /// <summary>
    /// Assignment utility
    /// </summary>
    public class AssignmentUtility : IAssignmentUtility
    {
        private readonly Action<object, object> _updateHandler;

        public static IAssignmentUtility Build(Action<IAssignmentUtilityHelper> option)
        {
            var payload = AssignmentUtilityHelper.New();

            option(payload);

            return new AssignmentUtility(payload.Profiles);
        }

        /// <summary>
        /// Load mapper profiles from given list
        /// </summary>
        /// <param name="profiles"></param>
        private AssignmentUtility(IReadOnlyList<IAssignmentProfile> profiles)
        {
            void UpdateHandlerAction(object entity, object dto)
            {
                var mapper = profiles.FirstOrDefault(y => y.TypeCheck(entity)) ??
                             throw new Exception("Failed to find a matching profile");

                mapper.ResolveAssignment(profiles, entity, dto);
            }

            _updateHandler = UpdateHandlerAction;
        }

        public void Update<T>(T entity, T dto)
        {
            _updateHandler(entity, dto);
        }
    }
}