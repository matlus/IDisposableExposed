using DomainLayer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DisposableExample
{
    public sealed class TimeIntervalService : IHostedService, IAsyncDisposable
    {
        private readonly Task _completedTask = Task.CompletedTask;
        private readonly ILogger _logger;
        private Timer _timer;
        private bool _disposed;

        public TimeIntervalService(DomainFacade domainFacade, ILogger logger)
        {
            _logger = logger;            
        }

        private void ExecuteJob(object state)
        {
            _logger.LogInformation("Called ExecuteJob in TimeIntervalService");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ExecuteJob, state: null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return _completedTask;
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, 0);
            return _completedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        private async ValueTask DisposeAsync(bool disposing)
        {
            if (disposing && !_disposed)
            {
                await _timer.DisposeAsync();
            }

            _disposed = true;
        }
    }
}
