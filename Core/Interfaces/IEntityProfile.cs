using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityUpdater.Interfaces
{
    public interface IEntityProfile
    {
        void ResolveAssignment(IEnumerable<IEntityProfile> profiles, object entity, object dto);
        
        bool TypeCheck(object instance);

        Type Type { get; }

        MethodInfo ComparerMethodInfo { get; }
    }
}