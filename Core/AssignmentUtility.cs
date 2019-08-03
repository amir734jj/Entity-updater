using System;
using System.Linq;
using System.Reflection;
using EntityUpdater.Interfaces;

namespace EntityUpdater
{
    /// <summary>
    /// Assignment utility
    /// </summary>
    public class AssignmentUtility : IAssignmentUtility
    {
        private Action<object, object> _updateHandler;

        /// <summary>
        /// Load mapper profiles from assemblies
        /// </summary>
        /// <param name="assemblies"></param>
        public AssignmentUtility(params Assembly[] assemblies)
        {
            var classMapType = typeof(IAssignmentProfile);

            var assignmentMapProfiles = assemblies.SelectMany(assembly => assembly.DefinedTypes
                .Where(x => x.IsClass && !x.IsAbstract && classMapType.IsAssignableFrom(x))
                .Select(x => (IAssignmentProfile) Activator.CreateInstance(x)));

            InitializeUpdateHandler(assignmentMapProfiles.ToArray());
        }

        /// <summary>
        /// Load mapper profiles manually
        /// </summary>
        /// <param name="assignmentProfiles"></param>
        public AssignmentUtility(params IAssignmentProfile[] assignmentProfiles)
        {
            InitializeUpdateHandler(assignmentProfiles);
        }

        /// <summary>
        /// Initialize the update handler function
        /// </summary>
        /// <param name="assignmentProfiles"></param>
        private void InitializeUpdateHandler(params IAssignmentProfile[] assignmentProfiles)
        {
            void UpdateHandlerAction(object entity, object dto) =>
                (assignmentProfiles.FirstOrDefault(y => y.TypeCheck(entity)) != null
                    ? assignmentProfiles.FirstOrDefault(y => y.TypeCheck(entity))
                    : throw new Exception("Failed to find a matching profile"))?.ResolveAssignment(entity, dto);

            _updateHandler = UpdateHandlerAction;
        }

        public void Update<T>(T entity, T dto)
        {
            _updateHandler(entity, dto);
        }
    }
}