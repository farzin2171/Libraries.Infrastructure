using LibraryTools;
using LibraryTools.Types;
using WT.Logging.ApplicationInsights;
using WT.Logging.Graylog;
using WT.Logging.LoggingService;
using WT.Logging.Seq;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.SystemConsole.Themes;
using System;

namespace WT.Logging
{
    public static class Extensions
    {
        /// <summary>
        /// Extension to add properties to all logs within a scoped Http request
        /// </summary>
        /// <param name="app">your app builder stack</param>
        /// <returns>your app builder stack with injected logger enricher middleware</returns>
        public static IApplicationBuilder UseHttpLoggerEnricher(this IApplicationBuilder app)
        {
            app.UseMiddleware<HttpLoggerEnricherMiddleware>();

            return app;
        }

        /// <summary>
        /// Configure logging with Serilog.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>The <see cref="IHostBuilder"/>.</returns>
	    public static IHostBuilder UseLogging(this IHostBuilder builder)
        {
            builder.UseSerilog((hostingContext, services, loggerConfiguration) =>
            {
                var productInformation = new ProductInformation();
                var options = hostingContext.Configuration.GetOptions<LoggerOptions>("logger");

                var defaultLoggerEnricherOptions = new DefaultLoggerEnricherOptions
                {
                    Application = productInformation.Name,
                    ApplicationVersion = productInformation.Version,
                    ApplicationInformationalVersion = productInformation.InformationalVersion,
                    Division = options.Division,
                    Environment = options.Environment
                };

                var minimumLevel = hostingContext.HostingEnvironment.IsDevelopment()
                    ? LogEventLevel.Debug
                    : LogEventLevel.Information;

                loggerConfiguration
                    .MinimumLevel.ControlledBy(new LoggingLevelSwitch(minimumLevel))
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LoggingLevelSwitches.EntityFrameworkLevelSwitch);

                loggerConfiguration
                    .Enrich.WithMachineName()
                    .Enrich.WithExceptionDetails()
                    .Enrich.FromLogContext()
                    .Enrich.With(new DefaultLoggerEnricher(defaultLoggerEnricherOptions));

                // write to console if in development mode or running in container
                if (hostingContext.HostingEnvironment.IsDevelopment() ||
                    hostingContext.Configuration.GetDeploymentMode() == DeploymentMode.Containers)
                {
                    loggerConfiguration.WriteTo.Console(theme: AnsiConsoleTheme.Code);
                }

                AddLoggingService(loggerConfiguration, options.Service);
                AddApplicationInsightsService(loggerConfiguration, options.ApplicationInsights, services);
                AddGraylog(loggerConfiguration, options.Graylog);
                AddSeq(loggerConfiguration, options.Seq);
            });

            builder.ConfigureServices(services =>
            {
                services.AddLogging();
            });

            return builder;
        }

        private static void AddLoggingService(LoggerConfiguration loggerConfiguration, LoggingServiceLoggerOptions loggerOptions)
        {
            if (!loggerOptions.Enabled)
            {
                return;
            }

            if (string.IsNullOrEmpty(loggerOptions.ApiKey) ||
                string.IsNullOrEmpty(loggerOptions.ClientId) ||
                string.IsNullOrEmpty(loggerOptions.Endpoint))
            {
                StartupLogger.Logger.Error("Logging was configured with Logging Service enabled but one of the following values are missing: ApiKey, ClientId, Endpoint");
                return;
            }

            var httpLoggingClient = new HttpLoggingClient(loggerOptions.ApiKey, loggerOptions.ClientId);
            loggerConfiguration.WriteTo.Http(loggerOptions.Endpoint, queueLimitBytes: null, httpClient: httpLoggingClient, textFormatter: new CompactJsonFormatter());
            StartupLogger.Logger.Information("Logs will be sent to the Logging Service");
        }

        private static void AddApplicationInsightsService(LoggerConfiguration loggerConfiguration, ApplicationInsightsLoggerOptions loggerOptions, IServiceProvider services)
        {
            if (!loggerOptions.Enabled)
            {
                return;
            }

            var telemetryConfiguration = services.GetService<TelemetryConfiguration>();

            if (telemetryConfiguration == null)
            {
                StartupLogger.Logger.Error("Logging was configured with Application Insight enabled but the Application Insight module is not configured. Make sure that an instrumentation key was present.");
                return;
            }

            loggerConfiguration.WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces);
            StartupLogger.Logger.Information("Logs will be sent to Application Insights");
        }

        private static void AddGraylog(LoggerConfiguration loggerConfiguration, GraylogLoggerOptions loggerOptions)
        {
            if (!loggerOptions.Enabled)
            {
                return;
            }

            if (string.IsNullOrEmpty(loggerOptions.Hostname))
            {
                StartupLogger.Logger.Error("Logging was configured with Graylog enabled but one of the following values are missing: Hostname, Port");
                return;
            }

            loggerConfiguration.Enrich.With(new GraylogLoggerEnricher());

            var graylogSinkOptions = new GraylogSinkOptions
            {
                HostnameOrAddress = loggerOptions.Hostname,
                Port = loggerOptions.Port
            };


            loggerConfiguration.WriteTo.Graylog(graylogSinkOptions);
            StartupLogger.Logger.Information("Logs will be sent to Graylog");
        }

        private static void AddSeq(LoggerConfiguration loggerConfiguration, SeqLoggerOptions loggerOptions)
        {
            if (!loggerOptions.Enabled)
            {
                return;
            }

            if (string.IsNullOrEmpty(loggerOptions.Hostname))
            {
                StartupLogger.Logger.Error("Logging was configured with Seq enabled but one of the following values are missing: Hostname");
                return;
            }

            loggerConfiguration.WriteTo.Seq(loggerOptions.Hostname, apiKey: loggerOptions.ApiKey);
            StartupLogger.Logger.Information("Logs will be sent to Seq");
        }
    }
}
