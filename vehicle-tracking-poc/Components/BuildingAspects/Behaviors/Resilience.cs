using DomainModels.System;
using DomainModels.Types;
using Polly;
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

        public static FallbackPolicy<T> Fallback<T>(T defaultValue) =>
            Policy<T>
            .Handle<Exception>().Fallback(()=> defaultValue, (delegateReq) =>
            {
                //TODO: log , then call fallback action
            });

        public static TimeoutPolicy DefualtTimeout =>
            Policy
            .Timeout(TimeSpan.FromSeconds(Identifiers.TimeoutInSec));
    }
}
