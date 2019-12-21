using System;

namespace Core.Tests.Interfaces
{
    public interface IPerson
    {
        Guid Id { get; }
        
        string Firstname { get; set; }
        
        string Lastname { get; set; }
        
        int Age { get; set; }
        
        DateTime DateOfBirth { get; set; }
        
        bool IsPresent { get; set; }
    }
}