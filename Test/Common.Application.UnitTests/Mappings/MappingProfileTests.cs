﻿using AutoMapper;

using Common.Application.Mappings;

using Xunit;

namespace Common.Application.UnitTests.Mappings
{
    public class MappingProfileTests
    {
        [Fact]
        public void GivenAllMappingConfigurationsFromApplicationAssembly_ThenMappingConfigurationsShouldBeValid()
        {
            // Arrange
            var configuration = new MapperConfiguration(config =>
            {
                config.AddProfile(new MappingProfile(typeof(MappingProfile).Assembly));
            });

            // Assert
            configuration.AssertConfigurationIsValid();
        }
    }
}
