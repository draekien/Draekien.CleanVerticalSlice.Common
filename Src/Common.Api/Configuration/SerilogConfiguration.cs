using System;
using System.IO;

using Common.Api.Extensions;

using Microsoft.Extensions.Configuration;

using Serilog;

namespace Common.Api.Configuration
{
    public static class SerilogConfiguration
    {
        /// <summary>
        /// Creates a logger based on appsettings configuration
        /// </summary>
        /// <example>
        /// "Serilog": {
        ///   "Using": [
        ///   "Serilog.Sinks.Console",
        ///   "Serilog.Sinks.Seq",
        ///   "Serilog.Enrichers.ClientInfo"],
        ///   "MinimumLevel": {
        ///   "Default": "Information",
        ///   "Override": {
        ///   "Microsoft": "Warning",
        ///   "System": "Warning"
        ///   }
        /// },
        ///   "Enrich": [
        ///   "FromLogContext",
        ///   "WithMachineName",
        ///   "WithThreadId",
        ///   "WithProcessId",
        ///   "WithThreadId",
        ///   "WithClientIp",
        ///   "WithClientAgent"
        ///       ],
        ///   "Properties": {
        ///       "Application": "WeatherForecast.Api"
        ///   }
        /// }
        /// </example>
        /// <returns>A configured <see cref="ILogger"/></returns>
        public static ILogger CreateLogger()
        {
            Serilog.Debugging.SelfLog.Enable(Console.WriteLine);
            IConfigurationRoot configuration = new ConfigurationBuilder()
                                               .SetBasePath(Directory.GetCurrentDirectory())
                                               .AddJsonFile("appsettings.json")
                                               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                                               .Build();

            return new LoggerConfiguration()
                   .ReadFrom.Configuration(configuration)
                   .Enrich.WithCorrelationIdHeader()
                   .CreateLogger();
        }
    }
}
