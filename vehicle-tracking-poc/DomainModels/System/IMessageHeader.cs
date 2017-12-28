using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.System
{
    public interface IMessageHeader
    {
        Guid ExecutionId { get; }
        Guid CorrelateId { get; }
        Int32 Timestamp { get; }
    }
}