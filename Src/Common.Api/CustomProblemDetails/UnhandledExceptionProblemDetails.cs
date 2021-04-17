using System;

using Hellang.Middleware.ProblemDetails;

using Microsoft.AspNetCore.Http;

namespace Common.Api.CustomProblemDetails
{
    public class UnhandledExceptionProblemDetails : StatusCodeProblemDetails
    {
        /// <inheritdoc />
        public UnhandledExceptionProblemDetails(Exception ex) : base(StatusCodes.Status500InternalServerError)
        {
            Detail = ex?.Message ?? "An unexpected error has occured";
        }
    }
}
