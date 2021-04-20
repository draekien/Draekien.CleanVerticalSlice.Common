using AutoMapper;

using Draekien.CleanVerticalSlice.Common.Application.Mappings;
using Draekien.CleanVerticalSlice.Common.TestUtils.Mappings;
using Xunit;

namespace Draekien.CleanVerticalSlice.Common.Application.UnitTests.Mappings
{
    public class MappingProfileTests
    {
        [Fact]
        public void GivenAllMappingConfigurationsFromApplicationAssembly_ThenMappingConfigurationsShouldBeValid()
        {
            // Arrange
            var configuration = new MapperConfiguration(config =>
            {
                config.AddProfile(new AutoMapperTestingProfile(typeof(MappingProfile).Assembly));
            });

            // Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
