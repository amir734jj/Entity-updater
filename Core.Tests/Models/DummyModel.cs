using System;

namespace Core.Tests.Models
{
    public class DummyModel : IEquatable<DummyModel>
    {
        public NestedDummyModel NestedDummyModel { get; set; }

        public CircularModel CircularModel { get; set; }

        public Guid Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public int Age { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool IsPresent { get; set; }

        public bool Equals(DummyModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return 
                   Id.Equals(other.Id) && Firstname == other.Firstname && Lastname == other.Lastname &&
                   Age == other.Age && DateOfBirth.Equals(other.DateOfBirth) && IsPresent == other.IsPresent;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DummyModel) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NestedDummyModel, CircularModel, Id, Firstname, Lastname, Age, DateOfBirth,
                IsPresent);
        }
    }
}