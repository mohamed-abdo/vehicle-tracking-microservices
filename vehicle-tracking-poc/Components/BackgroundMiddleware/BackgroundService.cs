using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundMiddleware
{
    public abstract class BackgroundService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private static object _syncObject = new object();
        public BackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        public IServiceProvider Services => _serviceProvider;

        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            // Store the task we're executing
            lock (_syncObject)
            {
                _executingTask = ExecuteAsync(_stoppingCts.Token);
                // If the task is completed then return it,
                // this will bubble cancellation and failure to the caller
                if (_executingTask.IsCompleted)
                {
                    return _executingTask;
                }
                // Otherwise it's running
                return Task.CompletedTask;
            }
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return Task.CompletedTask;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
                return Task.CompletedTask;
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public virtual void Dispose()
        {
            _stoppingCts.Cancel();
        }
    }
}
