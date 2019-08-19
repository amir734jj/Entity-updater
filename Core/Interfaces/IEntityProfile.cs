using System;
using System.Collections.Generic;

namespace EntityUpdater.Interfaces
{
    public interface IEntityProfile
    {
        string UpdatePropertyMethodName { get; }

        void ResolveAssignment(IEnumerable<IEntityProfile> profiles, object entity, object dto);
        
        bool TypeCheck(object instance);

        Type Type { get; }
        
        // ReSharper disable once UnusedMemberInSuper.Global
        object UpdateProperty<TPropertyValue>(TPropertyValue entityPropVal, TPropertyValue dtoPropVal);
    }
}