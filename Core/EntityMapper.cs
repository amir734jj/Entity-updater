using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using EntityUpdater.Extensions;
using EntityUpdater.Interfaces;
using EntityUpdater.Logic;

namespace EntityUpdater
{
    internal class EntityMapperHelper : IEntityMapperHelper
    {
        public IEntityMapperHelper Assembly(params string[] names)
        {
            var assemblies = names.Select(System.Reflection.Assembly.Load).ToArray();

            Assembly(assemblies);
            
            return this;
        }

        public IEntityMapperHelper Assembly(params Assembly[] assemblies)
        {
            var classMapType = typeof(IEntityProfile);

            var result = assemblies.SelectMany(assembly => assembly.DefinedTypes
                .Where(x => x.IsClass && !x.IsAbstract && classMapType.IsAssignableFrom(x))
                .Select(x => x.Instantiate<IEntityProfile>()));

            Profiles = Profiles.AddRange(result);
            
            return this;
        }

        public IEntityMapperHelper Profile<T>(T instance) where T : IEntityProfile
        {
            Profiles = Profiles.Add(instance);

            return this;
        }

        public IEntityMapperHelper Profile<T>() where T : IEntityProfile
        {
            Profiles = Profiles.Add(typeof(T).Instantiate<IEntityProfile>());

            return this;
        }

        public ImmutableList<IEntityProfile> Profiles { get; private set; }

        internal EntityMapperHelper()
        {
            Profiles = ImmutableList<IEntityProfile>.Empty;
        }
    }

    /// <summary>
    ///     Assignment utility
    /// </summary>
    public class EntityMapper : IEntityMapper
    {
        private readonly Action<Type, object, object> _updateHandler;

        public static IEntityMapper Build(Action<IEntityMapperHelper> option)
        {
            var payload = new EntityMapperHelper();

            option(payload);

            return new EntityMapper(payload.Profiles);
        }

        /// <summary>
        ///     Load mapper profiles from given list
        /// </summary>
        /// <param name="profiles"></param>
        private EntityMapper(IReadOnlyCollection<IEntityProfile> profiles)
        {
            var entityMapperLogic = new EntityMapperLogic(profiles);

            _updateHandler = (type, entity, dto) =>
            {
                var profile = profiles.FirstOrDefault(y => y.TypeCheck(entity)) ??
                              throw new Exception($"Failed to find a matching profile for type {type.Name}");

                entityMapperLogic.ResolveUpdate(profile)(entity, dto);
            };
        }

        public void Update<T>(T entity, T dto)
        {
            _updateHandler(typeof(T), entity, dto);
        }
    }
}