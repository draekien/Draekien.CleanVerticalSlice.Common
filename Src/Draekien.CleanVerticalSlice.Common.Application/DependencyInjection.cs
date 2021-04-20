using System;
using System.Collections.Generic;
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
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddCommonApplication(this IServiceCollection services, ICollection<Assembly> assemblies)
        {
            if (assemblies is null) throw new ArgumentNullException(nameof(assemblies));

            assemblies.Add(Assembly.GetExecutingAssembly());

            services.AddAutoMapper(assemblies);
            MappingProfile.ConsumingApplicationAssemblies = assemblies;

            services.AddValidatorsFromAssemblies(assemblies);
            services.AddMediatR(assemblies, cfg => cfg.AsScoped());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        }
    }
}
