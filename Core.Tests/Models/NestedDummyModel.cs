using System;

namespace Core.Tests.Models
{
    public class NestedDummyModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool IsPresent { get; set; }
    }
}