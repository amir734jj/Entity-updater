using System;
using Core.Tests.Interfaces;

namespace Core.Tests.Models
{
    public class Person : IPerson, IEquatable<Person>
    {
        public Guid Id { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public int Age { get; set; }

        public DateTime DateOfBirth { get; set; }

        public bool IsPressent { get; set; }

        public bool Equals(Person other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id) && string.Equals(Firstname, other.Firstname) && string.Equals(Lastname, other.Lastname) && Age == other.Age && DateOfBirth.Equals(other.DateOfBirth) && IsPressent == other.IsPressent;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Person) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Firstname != null ? Firstname.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Lastname != null ? Lastname.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Age;
                hashCode = (hashCode * 397) ^ DateOfBirth.GetHashCode();
                hashCode = (hashCode * 397) ^ IsPressent.GetHashCode();
                return hashCode;
            }
        }
    }
}