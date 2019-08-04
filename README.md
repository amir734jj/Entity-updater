# Entity-updater
Update Entity from DTO using a simple mapper profile without using any reflection.

[![pipeline status](https://gitlab.com/hesamian/Entity-updater/badges/master/pipeline.svg)](https://gitlab.com/hesamian/Entity-updater/commits/master)

The idea is this library will generate assignments given specified properties using LINQ expressions:

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
```

### Build the utility:
```csharp
var utility = AssignmentUtility.Build(_ => {
    _.Assembly(Assembly.GetExecutingAssembly());
});

Person entity = ...;
Person dto = ...;

utility.Update(entity, dto);
```
