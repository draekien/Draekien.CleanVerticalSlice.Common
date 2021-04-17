using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Reflection;

using Draekien.CleanVerticalSlice.Common.Api.Configuration;
using Draekien.CleanVerticalSlice.Common.Api.CustomProblemDetails;
using Draekien.CleanVerticalSlice.Common.Api.Filters;

using FluentValidation.AspNetCore;

using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Converters;

using Serilog;

using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;

namespace Draekien.CleanVerticalSlice.Common.Api
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds Controller configuration with NewtonsoftJson, Adds HttpClient and HttpContextAccessor,
        /// Adds Problem Detail Maps, Adds Swashbuckle swagger doc generation
        /// </summary>
        /// <example>
        /// services.AddCommonApi(HostEnvironment, typeof(Startup).Namespace, new[] { typeof(Startup).Assembly, typeof(Application.DependencyInjection).Assembly });
        /// </example>
        /// <param name="services">The current <see cref="IServiceCollection"/></param>
        /// <param name="env">The current <see cref="IHostEnvironment"/></param>
        /// <param name="apiTitle">The title of the api document</param>
        /// <param name="assemblies">The assemblies that export xml comments</param>
        public static void AddCommonApi(
            this IServiceCollection services,
            IHostEnvironment env, string apiTitle,
            IEnumerable<Assembly> assemblies)
        {
            services.AddControllers(options =>
                    {
                        options.ReturnHttpNotAcceptable = true;
                        options.RespectBrowserAcceptHeader = true;

                        options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
                        options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));

                        options.Filters.Add(new ProducesResponseTypeAttribute(typeof(BadRequestProblemDetails), StatusCodes.Status400BadRequest));
                        options.Filters.Add(new ProducesResponseTypeAttribute(typeof(UnhandledExceptionProblemDetails), StatusCodes.Status500InternalServerError));
                        options.Conventions.Add(new ControllerNameFromGroupConvention());
                    })
                    .AddProblemDetailsConventions()
                    .AddFluentValidation(options =>
                    {
                        options.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                        options.AutomaticValidationEnabled = false;
                    })
                    .AddNewtonsoftJson(options =>
                    {
                        options.UseCamelCasing(true);
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    });
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddHealthChecks();

            services.AddProblemDetailMaps(env);

            services.AddSwaggerGen(c =>
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

            services.AddSwaggerExamplesFromAssemblies(Assembly.GetCallingAssembly());
            services.AddSwaggerGenNewtonsoftSupport();
        }

        /// <summary>
        /// Configures the application to use SwaggerUI, ReDoc, HealthChecks,
        /// SerilogRequestLogging, Problem Details, Routing, Authorization, and Endpoints
        /// </summary>
        /// <param name="app">The current <see cref="IApplicationBuilder"/></param>
        /// <param name="env">The current <see cref="IWebHostEnvironment"/></param>
        /// <param name="apiName">The name of the api</param>
        public static void UseCommonApi(
            this IApplicationBuilder app,
            IWebHostEnvironment env,
            string apiName)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", apiName));
                app.UseReDoc(c =>
                {
                    c.RoutePrefix = "docs";
                    c.DocumentTitle = apiName;
                    c.SpecUrl = "/swagger/v1/swagger.json";
                    c.ExpandResponses("200,201");
                    c.RequiredPropsFirst();
                    c.SortPropsAlphabetically();
                });
            }

            app.UseHealthChecks("/health");
            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (context, httpContext) =>
                {
                    context.Set("RequestHost", httpContext.Request.Host.Value);
                    context.Set("RequestScheme", httpContext.Request.Scheme);
                };
            });
            app.UseProblemDetails();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
