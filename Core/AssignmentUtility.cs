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

            Profiles = Profiles.AddRange(assemblies.SelectMany(assembly => assembly.DefinedTypes
                .Where(x => x.IsClass && !x.IsAbstract && classMapType.IsAssignableFrom(x))
                .Select(x => x.Instantiate<IAssignmentProfile>())));
        }

        public void Profile<T>(T instance) where T : IAssignmentProfile
        {
            Profiles = Profiles.Add(instance);
        }
        
        public void Profile<T>() where T : IAssignmentProfile
        {
            Profiles = Profiles.Add(typeof(T).Instantiate<IAssignmentProfile>());
        }
        
        public ImmutableList<IAssignmentProfile> Profiles { get; private set; } = ImmutableList<IAssignmentProfile>.Empty;
    }
    
    /// <summary>
    /// Assignment utility
    /// </summary>
    public class AssignmentUtility : IAssignmentUtility
    {
        private readonly Action<object, object> _updateHandler;

        public static IAssignmentUtility Build(Action<AssignmentUtilityHelper> option)
        {
            var payload = new AssignmentUtilityHelper();
            
            option(payload);
            
            return new AssignmentUtility(payload.Profiles);
        }

        /// <summary>
        /// Load mapper profiles from given list
        /// </summary>
        /// <param name="profiles"></param>
        private AssignmentUtility(IReadOnlyList<IAssignmentProfile> profiles)
        {
            void UpdateHandlerAction(object entity, object dto) =>
                (profiles.FirstOrDefault(y => y.TypeCheck(entity)) != null
                    ? profiles.FirstOrDefault(y => y.TypeCheck(entity))
                    : throw new Exception("Failed to find a matching profile"))?.ResolveAssignment(entity, dto);

            _updateHandler = UpdateHandlerAction;
        }

        public void Update<T>(T entity, T dto)
        {
            _updateHandler(entity, dto);
        }
    }
}