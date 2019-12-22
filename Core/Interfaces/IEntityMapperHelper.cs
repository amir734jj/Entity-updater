using System.Collections.Immutable;
using System.Reflection;

namespace EntityUpdater.Interfaces
{
    public interface IEntityMapperHelper
    {
        ImmutableList<IEntityProfile> Profiles { get; }
        
        IEntityMapperHelper Assembly(params string[] names);

        IEntityMapperHelper Assembly(params Assembly[] assemblies);

        IEntityMapperHelper Profile<T>(T instance) where T : IEntityProfile;

        IEntityMapperHelper Profile<T>() where T : IEntityProfile;
    }
}