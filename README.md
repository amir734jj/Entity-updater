# Entity-updater
Update Entity from DTO using a simple mapper profile without using any reflection.

[![pipeline status](https://gitlab.com/hesamian/Entity-updater/badges/master/pipeline.svg)](https://gitlab.com/hesamian/Entity-updater/commits/master)

[NuGet](https://www.nuget.org/packages/Entity-updater/)

This library is useful to update "Entities" (special objects that are generated from EntityFramework) from
DTO (Data-Transfer-Object or objects that are coming from API layer) when Entity and DTO have the same type.
This library will generate assignments given specified properties using LINQ expression trees:

```csharp
entity.Id = dto.Id;

entity.Firstname = dto.Firstname;

entity.Lastname = dto.Lastname;

entity.DateOfBirth = dto.DateOfBirth;

entity.IsPressent = dto.IsPressent;
```

To use the library:

### Person POCO:
```csharp
public class Person
{
    public Guid Id { get; set; }

    public string Firstname { get; set; }

    public string Lastname { get; set; }

    public int Age { get; set; }

    public DateTime DateOfBirth { get; set; }

    public bool IsPressent { get; set; }
}
```

### Mapper profile:
```csharp
public class PersonProfile : EntityProfile<Person>
{
    public PersonProfile()
    {
        Map(x => x.Id)
            .Then(x => x.Age)
            .Then(x => x.Firstname)
            .Then(x => x.Lastname)
            .Then(x => x.IsPressent)
            .Then(x => x.DateOfBirth);
    }
}
```

### Build the utility:
```csharp
var utility = EntityMapper.Build(_ => {
    _.Assembly(Assembly.GetExecutingAssembly());
});

Person entity = ...;
Person dto = ...;

utility.Update(entity, dto);
```
