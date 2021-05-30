using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Draekien.CleanVerticalSlice.Common.Api.CustomProblemDetails;
using Draekien.CleanVerticalSlice.Common.Api.Filters;
using Draekien.CleanVerticalSlice.Common.Application.Exceptions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;

namespace Draekien.CleanVerticalSlice.Common.Api.Configuration.DependencyInjection
{
    /// <summary>
    /// Dependency Injection methods for adding CleanVerticalSlice.Common.Api
    /// </summary>
    public static class CleanVerticalSliceServiceCollectionExtensions
    {
        /// <summary>
        /// Creates a new CleanVerticalSlice.Common.Api builder.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>The builder.</returns>
        public static ICleanVerticalSliceBuilder AddCommonApiBuilder(this IServiceCollection services)
            => new CleanVerticalSliceCommonApiBuilder(services);

        /// <summary>
        /// Adds the default configuration for the API project
        /// </summary>
        /// <param name="builder">The service builder.</param>
        /// <param name="apiTitle">The title of the API.</param>
        /// <param name="environment">The host environment.</param>
        /// <param name="assemblies">The assemblies generating XML documentation.</param>
        /// <param name="options">The custom problem detail maps.</param>
        /// <returns>The service builder.</returns>
        public static ICleanVerticalSliceBuilder AddCommonApi(
            this ICleanVerticalSliceBuilder builder,
            string apiTitle,
            IHostEnvironment environment,
            ICollection<Assembly> assemblies,
            Action<ProblemDetailsOptions>? options = null)
        {
            builder.AddControllers()
                   .AddProblemDetailMaps(environment, options)
                   .AddSwaggerDocumentation(apiTitle, assemblies);

            return builder;
        }

        /// <summary>
        /// Adds the default controller configuration.
        /// </summary>
        /// <remarks>
        /// Only required if <see cref="AddCommonApi"/> is not invoked.
        /// </remarks>
        /// <param name="builder">The service builder.</param>
        /// <returns>The service builder.</returns>
        public static ICleanVerticalSliceBuilder AddControllers(this ICleanVerticalSliceBuilder builder)
        {
            builder.Services
                   .AddControllers(ControllerOptions.ConfigureMvcOptions)
                   .AddProblemDetailsConventions()
                   .AddFluentValidation(ControllerOptions.ConfigureMvcFluentValidation)
                   .AddNewtonsoftJson(ControllerOptions.ConfigureMvcNewtonsoftJson);

            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHealthChecks();

            return builder;
        }

        /// <summary>
        /// Adds the default swagger documentation configuration.
        /// </summary>
        /// <remarks>
        /// Only required if <see cref="AddCommonApi"/> is not invoked.
        /// </remarks>
        /// <param name="builder">The service builder.</param>
        /// <param name="apiTitle">The title of the API.</param>
        /// <param name="assemblies">The assemblies generating XML documentation.</param>
        /// <returns>The service builder.</returns>
        public static ICleanVerticalSliceBuilder AddSwaggerDocumentation(
            this ICleanVerticalSliceBuilder builder,
            string apiTitle,
            ICollection<Assembly> assemblies)
        {
            builder.Services
                   .AddSwaggerGen(c =>
                   {
                       c.SwaggerDoc("v1", new OpenApiInfo { Title = apiTitle, Version = "v1" });

                       foreach (Assembly assembly in assemblies)
                       {
                           var xmlFile = $"{assembly.GetName().Name}.xml";
                           string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                           c.IncludeXmlComments(xmlPath, true);
                       }

                       c.EnableAnnotations();

                       c.DocInclusionPredicate((_,_) => true);
                       c.AddFluentValidationRules();

                       c.ExampleFilters();
                       c.OperationFilter<AddCorrelationIdHeaderParameter>();
                   });

            builder.Services.AddSwaggerExamplesFromAssemblies(assemblies.ToArray());
            builder.Services.AddSwaggerGenNewtonsoftSupport();

            return builder;
        }

        /// <summary>
        /// Adds the default Problem Detail mappings.
        /// </summary>
        /// <remarks>
        /// Only required if <see cref="AddCommonApi"/> is not invoked.
        /// </remarks>
        /// <param name="builder">The service builder.</param>
        /// <param name="environment">The host environment.</param>
        /// <param name="problemDetailsOptions">The problem details options.</param>
        /// <returns>The service builder.</returns>
        public static ICleanVerticalSliceBuilder AddProblemDetailMaps(
            this ICleanVerticalSliceBuilder builder,
            IHostEnvironment environment,
            Action<ProblemDetailsOptions>? problemDetailsOptions)
        {
            if (problemDetailsOptions is null)
            {
                builder.Services
                       .AddProblemDetails(
                           options =>
                           {
                               options.OnBeforeWriteDetails = (context, details) => details.Instance = Activity.Current?.Id ?? context.TraceIdentifier;
                               options.IncludeExceptionDetails = (_, _) => environment.IsDevelopment();

                               options.Map<ValidationException>(ex => new BadRequestProblemDetails(ex));
                               options.Map<InvalidOperationException>(ex => new BadRequestProblemDetails(ex));
                               options.Map<ArgumentOutOfRangeException>(ex => new BadRequestProblemDetails(ex));
                               options.Map<NotFoundException>(ex => new NotFoundProblemDetails(ex));
                               options.Map<Exception>(ex => new UnhandledExceptionProblemDetails(ex));
                           }
                       );

                return builder;
            }

            builder.Services.AddProblemDetails(problemDetailsOptions);

            return builder;
        }
    }
}
