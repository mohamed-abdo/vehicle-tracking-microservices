using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Types.Messages
{
    public interface IMessageHeader
    {
        string ExecutionId { get; }
        string CorrelationId { get; }
        long Timestamp { get; }
    }
}