using System;
using System.Collections.Generic;

namespace EntityUpdater.Interfaces
{
    public interface IAssignmentProfile
    {
        string UpdatePropertyMethodName { get; }

        void ResolveAssignment(IEnumerable<IAssignmentProfile> profiles, object entity, object dto);
        
        bool TypeCheck(object instance);

        Type Type { get; }
        
        // ReSharper disable once UnusedMemberInSuper.Global
        object UpdateProperty<TPropertyValue>(TPropertyValue entityPropVal, TPropertyValue dtoPropVal);
    }
}