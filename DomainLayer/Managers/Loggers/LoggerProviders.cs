using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace DomainLayer
{
    public sealed class LoggerProvider : IDisposable
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly string eventLogName;
        private readonly ITelemetryChannel telemetryChannel;
        private readonly ConcurrentDictionary<string, ILogger> loggersDictionary = new ConcurrentDictionary<string, ILogger>();
        private bool _disposed;

        public LoggerProvider(string eventLogName, Func<IConfiguration> loggingConfigurationDelegate, string appInsightsInstrumentationKey = null)
        {
            this.eventLogName = eventLogName;

            telemetryChannel = new InMemoryChannel();
            loggerFactory = new LoggerFactory();
            loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                .ClearProviders()
                .AddConfiguration(loggingConfigurationDelegate())
#if DEBUG
                .AddDebug()
#endif
                .AddConsole()
                .AddEventSourceLogger();
                /*
                 * If the appInsightsInstrumentationKey is null/Empty, no problem. No Application
                 * Insights logging will occur. So local machine doesn't need this setting.
                 * When deployed to Azure, need to ensure ApplicationInsights:InstrumentationKey
                 * setting is present and valid
                */
                builder.Services.Configure<TelemetryConfiguration>(telemetryConfiguration =>
                {
                    telemetryConfiguration.TelemetryChannel = telemetryChannel;
                    telemetryConfiguration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());
                    telemetryConfiguration.InstrumentationKey = appInsightsInstrumentationKey;
                });

                builder.AddApplicationInsights();

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    /*
                     * Before you can see EventLog entries in the Windows EventViewer, you need to
                     * Create the LogName and Source using Powershell as an Administrator.
                     * Use: New-EventLog -LogName eventLogName -Source eventLogName                    
                     * Where eventLogName is the actual name such as "MyAppLog"
                     * To Remove use: Remove-EventLog -LogName eventLogName   
                     */
                    builder.AddEventLog(new EventLogSettings { LogName = eventLogName, SourceName = eventLogName });
                }
            });
        }

        public ILogger CreateLogger()
        {
            return loggersDictionary.GetOrAdd(eventLogName, loggerFactory.CreateLogger(eventLogName));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                telemetryChannel.Flush();
                loggerFactory.Dispose();                
                telemetryChannel.Dispose();
            }

            _disposed = true;
        }
    }
}