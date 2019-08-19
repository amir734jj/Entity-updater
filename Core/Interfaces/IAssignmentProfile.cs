using System;
using System.Collections.Generic;

namespace EntityUpdater.Interfaces
{
    public interface IAssignmentProfile
    {
        string UpdatePropertyMethodName { get; }

        void ResolveAssignment(IReadOnlyList<IAssignmentProfile> profiles, object entity, object dto);
        
        bool TypeCheck(object instance);

        Type Type { get; }
        
        object UpdateProperty<T>(T entityPropVal, T dtoPropVal);
    }
}