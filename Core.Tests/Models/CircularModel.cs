using System;

namespace Core.Tests.Models
{
    public class CircularModel
    {
        public Guid Id { get; set; }
        
        public CircularModel CircularModelRef { get; set; }
    }
}