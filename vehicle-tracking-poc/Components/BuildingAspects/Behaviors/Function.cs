using DomainModels.Types;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Threading.Tasks;

namespace BuildingAspects.Behaviors
{
    public sealed class Function
    {
        #region internal fields

        private const int _waitTimeSpanInSec = 1;
        private readonly ILogger _logger;
        private readonly int _retryCount;
        public Function(ILogger logger, int retryCount = 3)
        {
            _logger = logger;
            _retryCount = retryCount;
        }

        Action<Exception> logCritical => (e) => _logger?.LogCritical(e, e.Message);
        Action<string> logInfo => (message) => _logger?.LogInformation(message);

        RetryPolicy<T> defaultPolicy<T>(Func<T> action)
        {
            return Policy<T>
                   .HandleResult(result => ((result is IOptional r) ? !r.Optional : true) || result != null)
                   .Or<Exception>()
                   .WaitAndRetry(_retryCount,
                                   sleepDurationProvider: (i, result, context) =>
                                   {
                                       logInfo($"Info-Now: {DateTime.UtcNow}. System Waiting for {_waitTimeSpanInSec} seconds, then retry of {i} for {_retryCount}; Execution Id {context.ExecutionGuid}.");
                                       return TimeSpan.FromSeconds(_waitTimeSpanInSec);
                                   },
                                   onRetry: (result, timespan, i, context) =>
                                   {
                                       logCritical(result.Exception);
                                   });
            //TODO:enable the following feature after analyze performance impact, since while debugging i notice some delay.
            //fall-back wrapper will be in action when exception occur.
            //TODO: enrich fallback feature, ... action from the caller.
            //.Wrap(Resilience.Fallback(fallbackValue))
            //TODO enrich timeout feature, with more actions
            //.Wrap(Resilience.DefualtTimeout);
        }

        #endregion

        public T Decorate<T>(Func<T> action)
        {
            return
                defaultPolicy(action)
                .Execute(action);
        }

        public Task<T> DecorateTask<T>(Func<T> action)
        {
            return
                defaultPolicy(action)
                .ExecuteAsync(() => Task.FromResult(action()));
        }

    }
}
