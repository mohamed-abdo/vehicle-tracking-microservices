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

        private const int _waitTimeSpanInSec = 5;
        private readonly ILogger _logger;
        private readonly int _retryCount;
        public Function(ILogger logger, int retryCount = 3)
        {
            _logger = logger;
            _retryCount = retryCount;
        }

        Action<Exception> logCritical => (e) => _logger?.LogCritical(e, e.Message);
        Action<string> logInfo => (message) => _logger?.LogInformation(message);

        RetryPolicy<T> defaultPolicy<T>(Func<T> action, Func<Exception, bool> exceptionPredicate = null)
        {
            Func<Exception, bool> defaultExHandler = (ex) => false;
            //TODO:exception type pattern matching with relevant behavior / policy.
            return Policy<T>
                   // in case of T implementing IOptional (this type shouldn't allow null), and instance of T is null, consider as exception
                   .HandleResult(result => ((result is IOptional r) ? !r.IsOptional : false) && result == null)
                   .Or(exceptionPredicate ?? defaultExHandler)
                   .WaitAndRetry(_retryCount,
                                   sleepDurationProvider: (i, result, context) =>
                                   {
                                       
                                       logInfo($"Info-Now: {DateTime.UtcNow}. System is waiting for {_waitTimeSpanInSec} seconds, then retry of {i} for {_retryCount}; Execution Id {context.ExecutionGuid}.");
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

        public Task<T> Decorate<T>(Func<T> action, Func<Exception, bool> exceptionPredicate = null)
        {
            return Task.FromResult(defaultPolicy(action).Execute(action));
        }

    }
}
