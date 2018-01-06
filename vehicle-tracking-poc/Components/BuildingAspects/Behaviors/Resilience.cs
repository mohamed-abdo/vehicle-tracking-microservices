using DomainModels.System;
using DomainModels.Types;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Timeout;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingAspects.Behaviors
{
    //TODO: build resilience profiles for normal, critical, ... behaviors
    public static class Resilience
    {
        /// <summary>
        /// Define general policy "behavior" for complex types to be by default not nullable"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Policy</returns>
        public static PolicyBuilder<T> Result<T>() =>
            Policy<T>
            .HandleResult(result => ((result is IOptional r) ? !r.Optional : true) || result != null);


        /// <summary>
        /// Circuit breaker behavior use cases in case to connect to a service over the network ex., repository, third party, cache server, messages broker.. 
        /// in particularly in case of unreliable provider.
        /// </summary>
        public static CircuitBreakerPolicy DefaultCircuitBreaker =>
            Policy
            .Handle<Exception>()
            .CircuitBreaker(Identifiers.CircutBreakerExceptionsCount, TimeSpan.FromSeconds(Identifiers.BreakTimeoutInSec),
                onBreak: (ex, breakTimeout) =>
                {
                    //TODO: log break exception, break timeout
                },
                onReset: () =>
                {
                    //TODO: log circuit has been braked, and do accordingly actions.
                });

        /// <summary>
        /// For critical execution, functions should provide a fallback mechanism "or a default result" to able to return expected results.
        /// A use case calling repository to get a data (not critical at this context), in case of failure, bring data from the cache,
        /// or in case can't get cached data, get from repository; hint: you should consider circuit breaker with fallback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static FallbackPolicy<T> Fallback<T>(T defaultValue) =>
            Policy<T>
            .Handle<Exception>().Fallback(() => defaultValue, (delegateReq) =>
             {
                 //TODO: log , then call fallback action
             });

        /// <summary>
        /// For network functions you should provide a timeout to avoid system halt.
        /// A use case calling repository to get a data (not critical at this context), in case of timeout, bring data from the cache,
        /// or in case can't get cached data, get from repository; hint: you should consider circuit breaker with timeout.
        /// </summary>
        public static TimeoutPolicy DefualtTimeout =>
            Policy
            .Timeout(TimeSpan.FromSeconds(Identifiers.TimeoutInSec));
    }
}
