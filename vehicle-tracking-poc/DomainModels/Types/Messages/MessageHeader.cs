﻿using System;

namespace DomainModels.Types.Messages
{
    [Serializable]
    public class MessageHeader : IMessageHeader
    {

        private readonly Guid _executionId;
        private Guid _correlationId;
        private readonly long _timestamp;
        private readonly bool _isSucceed;
        public MessageHeader(Guid? executionId = null, Guid? correlationId = null, long? timestamp = null, bool? isSucceed = null)
        {
            _executionId = executionId ?? Guid.NewGuid();
            _correlationId = correlationId ?? _executionId;
            _timestamp = timestamp ?? new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            _isSucceed = isSucceed ?? true;
        }

        public Guid ExecutionId => _executionId;
        public long Timestamp => _timestamp;
        public bool IsSucceed => _isSucceed;
        //TODO: utilize correlation id for building robust distributed messages.
        public Guid CorrelationId { get => _correlationId; set => _correlationId = value; }
    }
}