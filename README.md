# Entity-updater
Update Entity from DTO using a simple mapper profile without using any reflection

[![pipeline status](https://gitlab.com/hesamian/Entity-updater/badges/master/pipeline.svg)](https://gitlab.com/hesamian/Entity-updater/commits/master)


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
var utility = new AssignmentUtility(Assembly.GetExecutingAssembly());
// or var utility = new AssignmentUtility(new [] { new PersonAssignmentProfile() });

Person entity = ...;
Person dto = ...;

utility.Update(entity, dto);
```
