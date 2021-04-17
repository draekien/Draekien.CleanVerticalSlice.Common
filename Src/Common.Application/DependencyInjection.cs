using System.Reflection;

using Common.Application.Behaviours;
using Common.Application.Mappings;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Common.Application
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds AutoMapper profile, Fluent Validators, and MediatR Requests and Handlers
        /// </summary>
        /// <param name="services">The current <see cref="IServiceCollection"/></param>
        public static void AddCommonApplication(this IServiceCollection services)
        {
            var callingAssembly = Assembly.GetCallingAssembly();
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
