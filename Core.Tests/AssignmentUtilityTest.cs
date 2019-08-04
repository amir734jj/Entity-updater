using System.Reflection;
using AutoFixture;
using Core.Tests.Models;
using EntityUpdater;
using EntityUpdater.Interfaces;
using Xunit;

namespace Core.Tests
{
    public class AssignmentUtilityTest
    {
        private readonly Fixture _fixture;
        private readonly IAssignmentUtility _assignmentUtility;

        public AssignmentUtilityTest()
        {
            _fixture = new Fixture();

            _assignmentUtility = AssignmentUtility.Build(_ =>
            {
                _.Assembly(Assembly.GetExecutingAssembly());
            });
        }
        
        [Fact]
        public void Test__Mapper()
        {
            // Arrange
            var entity = _fixture.Create<Person>();
            var dto = _fixture.Create<Person>();

            _assignmentUtility.Update(entity, dto);

            // Act
            var rslt = entity.Equals(dto);

            // Assert
            Assert.True(rslt);
        }
    }
}