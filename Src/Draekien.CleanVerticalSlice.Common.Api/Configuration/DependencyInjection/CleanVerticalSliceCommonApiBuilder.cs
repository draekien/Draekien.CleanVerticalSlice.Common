using System;
using Microsoft.Extensions.DependencyInjection;

namespace Draekien.CleanVerticalSlice.Common.Api.Configuration.DependencyInjection
{
    /// <summary>
    /// CleanVerticalSlice.Common helper class for DependencyInjection configuration
    /// </summary>
    public class CleanVerticalSliceCommonApiBuilder : ICleanVerticalSliceBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CleanVerticalSliceCommonApiBuilder"/> class.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <exception cref="ArgumentNullException">services</exception>
        public CleanVerticalSliceCommonApiBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }
    }
}
