using System;
using System.Diagnostics;

using Common.Api.CustomProblemDetails;
using Common.Application.Exceptions;

using FluentValidation;

using Hellang.Middleware.ProblemDetails;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Api.Configuration
{
    public static class ProblemDetailsConfiguration
    {
        public static void AddProblemDetailMaps(this IServiceCollection services, IHostEnvironment env)
        {
            services.AddProblemDetails(options =>
            {
                options.OnBeforeWriteDetails = (context, details) => details.Instance = Activity.Current?.Id ?? context.TraceIdentifier;
                options.IncludeExceptionDetails = (_, _) => env.IsDevelopment();

                options.Map<ValidationException>(ex => new BadRequestProblemDetails(ex));
                options.Map<InvalidOperationException>(ex => new BadRequestProblemDetails(ex));
                options.Map<ArgumentOutOfRangeException>(ex => new BadRequestProblemDetails(ex));
                options.Map<NotFoundException>(ex => new NotFoundProblemDetails(ex));
                options.Map<Exception>(ex => new UnhandledExceptionProblemDetails(ex));
            });
        }
    }
}
