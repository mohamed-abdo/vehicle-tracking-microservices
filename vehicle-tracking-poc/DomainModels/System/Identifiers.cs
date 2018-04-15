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
        public const string MessageSubscriberRoutes = "middleware_routes_subscriber";
        public const string MessagePublisherRoute = "middleware_ping_publisher";
        public const string MessagesMiddlewareUsername = "middleware_username";
        public const string MessagesMiddlewarePassword = "middleware_password";
        public const string EventDbConnection = "event_db_connection";
        public const string DefaultJsonObject = "{}";
        #endregion

        #region settings

        public const int RetryCount = 5;
        public const int DataPageSize = 10;
        public const int TimeoutInSec = 60;
        public const int BreakTimeoutInSec = 5;
        public const int CircutBreakerExceptionsCount = 5;
        #endregion
    }
}
