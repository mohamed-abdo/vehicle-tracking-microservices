using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.System
{
    public sealed class Identifiers
    {
        #region infrastructure

        public const string CacheServer = "distributed_cache";
        public const string CacheDBVehicles = "cache_db_vehicles";
        public const string MessagesMiddleware = "messages_middleware";
        public const string MiddlewareExchange = "middleware_exchange";
        public const string MessageSubscriberRoute = "middleware_info_subscriber";
        public const string MessagePublisherRoute = "middleware_info_publisher";
        public const string MessagesMiddlewareUsername = "middleware_username";
        public const string MessagesMiddlewarePassword = "middleware_password";

        #endregion

        #region settings

        public const int RetryCount = 3;
        public const int TimeoutInSec = 30;
        #endregion
    }
}
