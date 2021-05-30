using System;

using Hellang.Middleware.ProblemDetails;

using Microsoft.AspNetCore.Http;

namespace Draekien.CleanVerticalSlice.Common.Api.CustomProblemDetails
{
    public class NotFoundProblemDetails : StatusCodeProblemDetails
    {
        /// <inheritdoc />
        public NotFoundProblemDetails(Exception ex) : base(StatusCodes.Status404NotFound)
        {
            Detail = string.IsNullOrWhiteSpace(ex.Message) ? "The requested resource was not found" : ex.Message;
        }
    }
}
