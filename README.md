# Clean Vertical Slice Common

- [About](#about)
- [Technology used](#technology-used)
- [API](#api)
- [Application](#application)
- [Usage](#usage)

## About

This repository contains the projects required for setting up the [Clean Vertical Slice Template](https://github.com/draekien/Draekien.CleanVerticalSlice.Template) solution. These projects contain the common logic required for setting up a new feature in the Clean Vertical Slice Architecture.

## Technology used

- [FluentValidation](https://fluentvalidation.net/)
- [Hellang.Middleware.ProblemDetails](https://github.com/khellang/Middleware)
- [MediatR](https://github.com/jbogard/MediatR)
- [Serilog](https://serilog.net/)
- [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [AutoMapper](https://docs.automapper.org/en/stable/Getting-started.html)

## API

The API project configures the following:

- MediatR enabled API Controller
- Configuration of Serilog from appsettings with support for correlation id via `x-correlation-id` header
- Mapping of common exceptions to problem details
- Configuration of swagger docs

## Application

The Application project configures the following for the calling assembly:

- Fluent validation rules
- MediatR pipeline for fluent validation
- AutoMapper profile configuration and `IMapFrom<T>` interface

## Usage

1. Install the Nuget Packages into your solution

   ```sh
   # In your API layer project
   dotnet add package Draekien.CleanVerticalSlice.Common.Api

   # In your Application layer project
   dotnet add package Draekien.CleanVerticalSlice.Common.Application
   ```

2. In your consuming API project, add Serilog configuration to `appsettings.json`

   ```json
   // example serilog configuration
   "Serilog": {
      "Using": [
      "Serilog.Sinks.Console",
      "Serilog.Sinks.Seq",
      "Serilog.Enrichers.ClientInfo"
      ],
      "MinimumLevel": {
         "Default": "Information",
         "Override": {
            "Microsoft": "Warning",
            "System": "Warning"
         }
      },
      "Enrich": [
      "FromLogContext",
      "WithMachineName",
      "WithThreadId",
      "WithProcessId",
      "WithThreadId",
      "WithClientIp",
      "WithClientAgent"
      ],
      "Properties": {
         "Application": "WeatherForecast.Api"
      }
   }
   ```

3. Add common api to your API's `Startup.cs`

   ```csharp
   // add IHostEnvironment to your DI container
   public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment) {
     Configuration = configuration;
     HostEnvironment = hostEnvironment;
   }

   public IConfiguration Configuration { get; }
   public IHostEnvironment HostEnvironment { get; }

   // in ConfigureServices
   services.AddCommonApiBuilder()
           .AddCommonApi(
              typeof(Startup).Namespace, // tne name of the API for swagger
              HostEnvironment,
              new[] { // the assemblies to injest XML documentation from
                 typeof(Startup).Assembly,
                 typeof(Application.DependencyInjection).Assembly
              }
           );

   // in Configure
   app.UseCommonApi(env, $"{typeof(Startup).Namepsace} v1");
   ```

4. Add common application to your Application's `DependencyInjection.cs`

   ```csharp
   public static void AddApplication(this IServiceCollection services)
   {
      services.AddCommonApplication(new[] { typeof(DependencyInjection).Assembly });
   }
   ```

5. Update your API and Application `csproj` files to generate xml documentation files

   ```xml
   <PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn>
   </PropertyGroup>
   ```
