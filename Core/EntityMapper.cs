using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using EntityUpdater.Extensions;
using EntityUpdater.Interfaces;

namespace EntityUpdater
{
    internal class EntityMapperHelper : IEntityMapperHelper
    {
        public void Assembly(params string[] names)
        {
            var assemblies = names.Select(System.Reflection.Assembly.Load).ToArray();

            Assembly(assemblies);
        }

        public void Assembly(params Assembly[] assemblies)
        {
            var classMapType = typeof(IEntityProfile);

            var result = assemblies.SelectMany(assembly => assembly.DefinedTypes
                .Where(x => x.IsClass && !x.IsAbstract && classMapType.IsAssignableFrom(x))
                .Select(x => x.Instantiate<IEntityProfile>()));

            Profiles = Profiles.AddRange(result);
        }

        public void Profile<T>(T instance) where T : IEntityProfile
        {
            Profiles = Profiles.Add(instance);
        }

        public void Profile<T>() where T : IEntityProfile
        {
            Profiles = Profiles.Add(typeof(T).Instantiate<IEntityProfile>());
        }

        public ImmutableList<IEntityProfile> Profiles { get; private set; }

        internal EntityMapperHelper()
        {
            Profiles = ImmutableList<IEntityProfile>.Empty;
        }
    }

    /// <summary>
    /// Assignment utility
    /// </summary>
    public class EntityMapper : IEntityMapper
    {
        private readonly Action<object, object> _updateHandler;

        public static IEntityMapper Build(Action<IEntityMapperHelper> option)
        {
            var payload = new EntityMapperHelper();

            option(payload);

            return new EntityMapper(payload.Profiles);
        }

        /// <summary>
        /// Load mapper profiles from given list
        /// </summary>
        /// <param name="profiles"></param>
        private EntityMapper(IReadOnlyList<IEntityProfile> profiles)
        {
            void UpdateHandlerAction(object entity, object dto)
            {
                var mapper = profiles.FirstOrDefault(y => y.TypeCheck(entity)) ??
                             throw new Exception("Failed to find a matching profile");

                // mapper.ResolveAssignment(profiles, entity, dto);
            }

            _updateHandler = UpdateHandlerAction;
        }

        public void Update<T>(T entity, T dto)
        {
            _updateHandler(entity, dto);
        }
    }
}