using System;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog.Core;
using Serilog.Events;

namespace Draekien.CleanVerticalSlice.Common.Api.Enrichers
{
    public class CorrelationIdHeader : ILogEventEnricher
    {
        private const string CorrelationIdPropertyName = "CorrelationId";
        private readonly string _headerKey;
        private readonly IHttpContextAccessor _contextAccessor;

        public CorrelationIdHeader(string headerKey) : this(headerKey, new HttpContextAccessor())
        { }

        internal CorrelationIdHeader(string headerKey, IHttpContextAccessor contextAccessor)
        {
            _headerKey = headerKey;
            _contextAccessor = contextAccessor;
        }

        /// <inheritdoc />
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_contextAccessor.HttpContext is null) return;

            string correlationId = GetCorrelationId(_contextAccessor.HttpContext);

            var correlationIdProperty = new LogEventProperty(CorrelationIdPropertyName, new ScalarValue(correlationId));

            logEvent.AddOrUpdateProperty(correlationIdProperty);
        }

        private string GetCorrelationId(HttpContext context)
        {
            var header = string.Empty;

            if (context.Request.Headers.TryGetValue(_headerKey, out StringValues values))
            {
                header = values.FirstOrDefault();
            }
            else if (context.Response.Headers.TryGetValue(_headerKey, out values))
            {
                header = values.FirstOrDefault();
            }

            string correlationId = string.IsNullOrWhiteSpace(header) ? Guid.NewGuid().ToString() : header;

            if (!context.Request.Headers.ContainsKey(_headerKey))
            {
                context.Request.Headers.Add(_headerKey, correlationId);
            }

            return correlationId;
        }
    }
}
