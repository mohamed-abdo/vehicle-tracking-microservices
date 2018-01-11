using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Types.Messages
{
    public class MessageHeader : IMessageHeader
    {

        private readonly Guid _ExecutionId;
        private readonly long _Timestamp;
        private readonly bool _isSucceed;
        public MessageHeader(Guid? executionId = null, long? timestamp = null, bool? isSucceed = null)
        {
            _ExecutionId = executionId ?? Guid.NewGuid();
            _Timestamp = timestamp ?? new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            _isSucceed = isSucceed ?? true;
        }

        public Guid ExecutionId => _ExecutionId;
        public long Timestamp => _Timestamp;
        public bool IsSucceed => _isSucceed;
        //TODO: utilize correlation id for building robust distributed messages.
        public Guid CorrelateId { get; set; }

    }
}