using Microsoft.Extensions.Logging;
using System;

namespace DomainLayer
{
    internal sealed class ServiceLocator : IDisposable
    {
        private readonly ConfigurationProvider _configurationProvider = new ConfigurationProvider();
        private LoggerProvider _loggerProvider;
        private bool _disposed;

        public ConfigurationProvider GetConfigurationProvider()
        {
            return _configurationProvider;
        }

        public ImdbServiceGateway GetImdbServiceGateway()
        {
            return new ImdbServiceGateway(GetConfigurationProvider().GetImdbServiceBaseUrl());
        }

        public ILogger GetLogger()
        {
            var configurationProvider = GetConfigurationProvider();
            _loggerProvider = _loggerProvider ?? new LoggerProvider("Some Event Log Name", configurationProvider.GetLoggingConfiguration, configurationProvider.GetAppInsightsInstrumentationKey());
            return _loggerProvider.CreateLogger();
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
                _loggerProvider?.Dispose();
            }

            _disposed = true;
        }
    }
}
