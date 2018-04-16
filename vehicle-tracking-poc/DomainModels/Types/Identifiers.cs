using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Types
{
    public sealed class Identifiers
    {
        public const string MessagePublisherRoute = "MessagePublisherRoute";
        public const string CorrelationId = "correlation-id";

        #region content Types

        public const string ApplicationJson = "application/json";
        #endregion
    }
}
