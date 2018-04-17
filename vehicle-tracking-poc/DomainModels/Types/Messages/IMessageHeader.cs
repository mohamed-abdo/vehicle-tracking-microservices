using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Types.Messages
{
    public interface IMessageHeader
    {
        Guid ExecutionId { get; }
        Guid CorrelationId { get; }
        long Timestamp { get; }
    }
}