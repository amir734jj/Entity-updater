using System;
using System.Collections.Generic;
using EntityUpdater.Abstracts;
using EntityUpdater.Interfaces;

namespace EntityUpdater.Models
{
    public class Vertex : IEquatable<Vertex>
    {
        public IEntityProfile Profile { get; private set; }

        public Vertex(IEntityProfile profile)
        {
            Profile = profile;
        }

        public bool Equals(Vertex other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Profile.Type == other.Profile.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Vertex) obj);
        }

        public override int GetHashCode()
        {
            return (Profile != null ? Profile.GetHashCode() : 0);
        }
    }
}