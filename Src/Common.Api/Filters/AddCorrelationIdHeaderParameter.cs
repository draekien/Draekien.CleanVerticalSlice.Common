using System;
using System.Collections.Generic;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.Api.Filters
{
    public class AddCorrelationIdHeaderParameter : IOperationFilter
    {
        /// <inheritdoc />
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "x-correlation-id",
                In = ParameterLocation.Header,
                Required = false,
                Description = "A Guid for correlating requests made to this API from other systems",
                Example = new OpenApiString(Guid.NewGuid().ToString())
            });
        }
    }
}
