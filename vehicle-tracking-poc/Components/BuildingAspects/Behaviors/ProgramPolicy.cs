using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BuildingAspects.Behaviors
{
    public class ProgramPolicy
    {
        private readonly ILogger _logger;
        private readonly int _retryCount;
        public ProgramPolicy(ILogger logger, int retryCount = 3)
        {
            _logger = logger;
            _retryCount = retryCount;
        }

        Action<Exception> logCritical => (e) => _logger?.LogCritical(e, e.Message);


        public T Decorate<T>(Func<T> action)
        {
            return Policy<T>
                    .HandleResult(result => result != null)
                    .Or<Exception>()
                    .Retry(_retryCount, (e, i) => logCritical(e.Exception))
                    .Execute(action);
        }

        public Task<T> DecorateTask<T>(Func<Task<T>> action)
        {
            return Policy<T>
                    .HandleResult(result => result != null)
                    .Or<Exception>()
                    .Retry(_retryCount, (e, i) => logCritical(e.Exception))
                    .ExecuteAsync(action);
        }

    }
}
