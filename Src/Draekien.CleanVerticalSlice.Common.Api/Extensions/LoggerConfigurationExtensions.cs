using System;

using Draekien.CleanVerticalSlice.Common.Api.Enrichers;

using Serilog;
using Serilog.Configuration;

namespace Draekien.CleanVerticalSlice.Common.Api.Extensions
{
    public static class LoggerConfigurationExtensions
    {
        public static LoggerConfiguration WithCorrelationIdHeader(this LoggerEnrichmentConfiguration configuration, string headerKey = "x-correlation-id")
        {
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            return configuration.With(new CorrelationIdHeader(headerKey));
        }
    }
}
