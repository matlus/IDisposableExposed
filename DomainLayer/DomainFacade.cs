using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainLayer
{
    public class DomainFacade : IDisposable
    {
        private readonly Manager _manager;
        private bool _disposed;

        public DomainFacade()
            :this(new ServiceLocator())
        {            
        }

        internal DomainFacade(ServiceLocator serviceLocator)
        {
            _manager = new Manager(serviceLocator);
        }

        public Task<IEnumerable<Movie>> GetAllMovies() => _manager.GetAllMovies();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _manager.Dispose();
            }

            _disposed = true;
        }
    }
}
