# Entity-updater
Update Entity from DTO using a simple mapper profile without using any reflection

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

var buld
```csharp


var assignmentUtility = new AssignmentUtility(Assembly.GetExecutingAssembly());
// or var assignmentUtility = new AssignmentUtility(new [] { new PersonAssignmentProfile() });



```
