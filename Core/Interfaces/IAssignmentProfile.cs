namespace EntityUpdater.Interfaces
{
    public interface IAssignmentProfile
    {
        void ResolveAssignment(object entity, object dto);
        
        bool TypeCheck(object instance);
    }
}