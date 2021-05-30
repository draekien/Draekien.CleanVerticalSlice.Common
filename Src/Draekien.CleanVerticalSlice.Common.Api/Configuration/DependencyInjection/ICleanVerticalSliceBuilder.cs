using Microsoft.Extensions.DependencyInjection;

namespace Draekien.CleanVerticalSlice.Common.Api.Configuration.DependencyInjection
{
    /// <summary>
    /// CleanVerticalSlice.Common builder interface
    /// </summary>
    public interface ICleanVerticalSliceBuilder
    {
        /// <summary>
        /// Gets the services collection
        /// </summary>
        IServiceCollection Services { get; }
    }
}
