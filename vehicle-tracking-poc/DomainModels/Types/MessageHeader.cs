using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Types
{
    public class MessageHeader : IMessageHeader
    {
        public MessageHeader()
        {
            _ExecutionId = Guid.NewGuid();
            _Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }

        private readonly Guid _ExecutionId;
        private readonly long _Timestamp;

        public Guid CorrelateId { get; set; }

        public Guid ExecutionId => _ExecutionId;

        public long Timestamp => _Timestamp;
    }
}