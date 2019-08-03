using Core.Tests.Models;
using EntityUpdater.Abstracts;

namespace Core.Tests.Profiles
{
    public class PersonAssignmentProfile : AssignmentProfile<Person>
    {
        public PersonAssignmentProfile()
        {
            Map(x => x.Id);
            Map(x => x.Age);
            Map(x => x.Firstname);
            Map(x => x.Lastname);
            Map(x => x.IsPressent);
            Map(x => x.DateOfBirth);
        }
    }
}