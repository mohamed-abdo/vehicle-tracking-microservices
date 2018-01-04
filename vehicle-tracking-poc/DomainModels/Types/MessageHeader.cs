using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Types
{
    public class MessageHeader : IMessageHeader
    {
        public Guid ExecutionId { get; set; }

        public Guid CorrelateId { get; set; }

        public int Timestamp { get; set; }
    }
}