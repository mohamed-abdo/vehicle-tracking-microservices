using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Types.Messages
{
    public class MessageHeader : IMessageHeader
    {

        private readonly Guid _executionId;
        private readonly long _timestamp;
        private readonly bool _isSucceed;
        public MessageHeader(Guid? executionId = null, long? timestamp = null, bool? isSucceed = null)
        {
            _executionId = executionId ?? Guid.NewGuid();
            _timestamp = timestamp ?? new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            _isSucceed = isSucceed ?? true;
        }

        public Guid ExecutionId => _executionId;
        public long Timestamp => _timestamp;
        public bool IsSucceed => _isSucceed;
        //TODO: utilize correlation id for building robust distributed messages.
        public Guid CorrelateId { get; set; }

    }
}