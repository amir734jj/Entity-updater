using System.Reflection;
using AutoFixture;
using Core.Tests.Models;
using EntityUpdater;
using EntityUpdater.Interfaces;
using Xunit;

namespace Core.Tests
{
    public class EntityMapperTest
    {
        private readonly Fixture _fixture;
        private readonly IEntityMapper _entityMapper;

        public EntityMapperTest()
        {
            _fixture = new Fixture();

            _entityMapper = EntityMapper.Build(_ =>
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

            _entityMapper.Update(entity, dto);

            // Act
            var rslt = entity.Equals(dto);

            // Assert
            Assert.True(rslt);
        }
    }
}