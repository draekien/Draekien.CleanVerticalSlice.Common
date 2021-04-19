using System.Reflection;

using Draekien.CleanVerticalSlice.Common.Application.Behaviours;
using Draekien.CleanVerticalSlice.Common.Application.Mappings;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Draekien.CleanVerticalSlice.Common.Application
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds AutoMapper profile, Fluent Validators, and MediatR Requests and Handlers
        /// </summary>
        /// <param name="services">The current <see cref="IServiceCollection"/></param>
        /// <param name="callingApplicationAssembly">The calling application assembly</param>
        public static void AddCommonApplication(this IServiceCollection services, Assembly callingApplicationAssembly = null)
        {
            Assembly callingAssembly = callingApplicationAssembly ?? Assembly.GetCallingAssembly();
            services.AddAutoMapper(config =>
            {
                config.AddProfile(new MappingProfile(callingAssembly));
            });
            services.AddValidatorsFromAssembly(callingAssembly);
            services.AddMediatR(callingAssembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        }
    }
}
