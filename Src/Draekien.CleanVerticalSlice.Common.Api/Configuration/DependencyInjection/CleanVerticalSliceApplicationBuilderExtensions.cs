using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Draekien.CleanVerticalSlice.Common.Api.Configuration.DependencyInjection
{
    /// <summary>
    /// Extension methods for using CleanVerticalSlice.Common.Api
    /// </summary>
    public static class CleanVerticalSliceApplicationBuilderExtensions
    {
        /// <summary>
        /// Registers swagger, health checks, Serilogger and problem details in the application builder.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The web host environment.</param>
        /// <param name="apiName">The name of the API.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseCommonApi(
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

            return app;
        }
    }
}
