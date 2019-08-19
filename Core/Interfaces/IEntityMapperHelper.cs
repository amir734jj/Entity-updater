using System.Collections.Immutable;
using System.Reflection;

namespace EntityUpdater.Interfaces
{
    public interface IEntityMapperHelper
    {
        ImmutableList<IEntityProfile> Profiles { get; }
        
        void Assembly(params string[] names);

        void Assembly(params Assembly[] assemblies);

        void Profile<T>(T instance) where T : IEntityProfile;

        void Profile<T>() where T : IEntityProfile;
    }
}