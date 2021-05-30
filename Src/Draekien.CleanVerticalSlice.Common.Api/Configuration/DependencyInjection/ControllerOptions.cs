using System.Net.Mime;
using Draekien.CleanVerticalSlice.Common.Api.CustomProblemDetails;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;

namespace Draekien.CleanVerticalSlice.Common.Api.Configuration.DependencyInjection
{
    internal static class ControllerOptions
    {
        public static void ConfigureMvcOptions(MvcOptions options)
        {
            options.ReturnHttpNotAcceptable = true;
            options.RespectBrowserAcceptHeader = true;

            options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
            options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));

            options.Filters.Add(new ProducesResponseTypeAttribute(typeof(BadRequestProblemDetails), StatusCodes.Status400BadRequest));
            options.Filters.Add(new ProducesResponseTypeAttribute(typeof(UnhandledExceptionProblemDetails), StatusCodes.Status500InternalServerError));
            options.Conventions.Add(new ControllerNameFromGroupConvention());
        }

        public static void ConfigureMvcFluentValidation(FluentValidationMvcConfiguration options)
        {
            options.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
            options.AutomaticValidationEnabled = false;
        }

        public static void ConfigureMvcNewtonsoftJson(MvcNewtonsoftJsonOptions options)
        {
            options.UseCamelCasing(true);
            options.SerializerSettings.Converters.Add(new StringEnumConverter());
        }
    }
}
