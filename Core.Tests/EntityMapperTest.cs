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
        private readonly IFixture _fixture;
        private readonly IEntityMapper _entityMapper;

        public EntityMapperTest()
        {
            _fixture = new Fixture();

            _entityMapper = EntityMapper.Build(_ =>
                _.Assembly(Assembly.GetExecutingAssembly()));

            _fixture.Customize<DummyModel>(x => x.With(y => y.CircularModel,
                () => _fixture.Build<CircularModel>().Without(y => y.CircularModelRef).Create()));
        }

        [Fact]
        public void Test__Mapper()
        {
            // Arrange
            var entity = _fixture.Create<DummyModel>();
            var dto = _fixture.Create<DummyModel>();

            // Act
            _entityMapper.Update(entity, dto);

            // Assert
            Assert.Equal(entity, dto);
        }
    }
}