using System.Collections.Immutable;
using System.Reflection;

namespace EntityUpdater.Interfaces
{
    public interface IAssignmentUtilityHelper
    {
        ImmutableList<IAssignmentProfile> Profiles { get; }
        
        void Assembly(params string[] names);

        void Assembly(params Assembly[] assemblies);

        void Profile<T>(T instance) where T : IAssignmentProfile;

        void Profile<T>() where T : IAssignmentProfile;
    }
}