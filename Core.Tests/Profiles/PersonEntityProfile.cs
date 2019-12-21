using Core.Tests.Models;
using EntityUpdater.Abstracts;

namespace Core.Tests.Profiles
{
    public class PersonEntityProfile : EntityProfile<Person>
    {
        public PersonEntityProfile()
        {
            Map(x => x.Id)
                .Then(x => x.Age)
                .Then(x => x.Firstname)
                .Then(x => x.Lastname)
                .Then(x => x.IsPresent)
                .Then(x => x.DateOfBirth)
                .Compare((x, y) => x.Id == y.Id);
        }
    }
}