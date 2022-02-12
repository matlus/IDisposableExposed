using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainLayer
{
    internal sealed class Manager : IDisposable
    {
        private readonly ServiceLocator _serviceLocator;
        private readonly ConfigurationProvider _configurationProvider;
        private readonly ILogger _logger;
        private readonly ImdbServiceGateway _imdbServiceGateway;
        private bool _disposed;

        public Manager(ServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
            _configurationProvider = _serviceLocator.GetConfigurationProvider();
            _logger = _serviceLocator.GetLogger();
            _imdbServiceGateway = _serviceLocator.GetImdbServiceGateway();
        }

        public Task<IEnumerable<Movie>> GetAllMovies()
        {
            _logger.LogInformation("Called GetAllMovies in Manager");
            return _imdbServiceGateway.GetAllMovies();
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
                _imdbServiceGateway.Dispose();
                _serviceLocator.Dispose();
            }

            _disposed = true;
        }
    }
}