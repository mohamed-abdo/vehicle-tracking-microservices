using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.System
{
    public abstract class InfrastructureConfiguration
    {
        public abstract InfrastructureConfiguration Create(IDictionary<string, string> configuration);
        #region fields
        public string CacheServer { get; protected set; }
        public string MessagesMiddleware { get; protected set; }
        public string CacheDBVehicles { get; protected set; }
        public string MiddlewareExchange { get; protected set; }
        public string MessageSubscriberRoute { get; protected set; }
        public string MessagePublisherRoute { get; protected set; }
        public string MessagesMiddlewareUsername { get; protected set; }
        public string MessagesMiddlewarePassword { get; protected set; }
        public string EventDbConnection { get; protected set; }
        #endregion
    }
}
