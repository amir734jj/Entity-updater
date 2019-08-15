namespace EntityUpdater.Interfaces
{
    public interface IAssignmentProfile
    {
        string UpdatePropertyMethodName { get; }

        void ResolveAssignment(object entity, object dto);
        
        bool TypeCheck(object instance);

        object UpdateProperty<T>(T entityPropVal, T dtoPropVal);
    }
}