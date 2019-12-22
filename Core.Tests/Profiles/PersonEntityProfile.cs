using System;
using Core.Tests.Models;
using EntityUpdater.Abstracts;

namespace Core.Tests.Profiles
{
    public class PersonEntityProfile : EntityProfile<Person>
    {
        public PersonEntityProfile()
        {
            Action<Person, Person> Update = (person1, person2) =>
            {
                // person1.Foo = person2.Foo;
               // person1.Foo = new Foo();
                person1.Foo.F1 = person2.Foo.F1;
                
                person1.Id = person1.Id;
                person1.Age = person2.Age;
                // person
            };
            
            MapPrimitive(x => x.Id)
                .MapReference(x => x.Foo)
                .MapPrimitive(x => x.Age)
                .MapRestrict(x => x.Firstname)
                .MapRestrict(x => x.Lastname)
                .MapPrimitive(x => x.IsPresent)
                .MapPrimitive(x => x.DateOfBirth)
                .Comparison((x, y) => x.Id == y.Id);
        }
    }
}