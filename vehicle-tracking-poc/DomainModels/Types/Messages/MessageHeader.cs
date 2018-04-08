using System;

namespace DomainModels.Types.Messages
{
    [Serializable]
    public class MessageHeader : IMessageHeader
    {

        private readonly string _executionId;
        private readonly long _timestamp;
        private readonly bool _isSucceed;
        public MessageHeader(string executionId = null, long? timestamp = null, bool? isSucceed = null)
        {
            _executionId = (executionId ?? Guid.NewGuid().ToString()).ToString();
            _timestamp = timestamp ?? new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            _isSucceed = isSucceed ?? true;
        }

        public string ExecutionId => _executionId;
        public long Timestamp => _timestamp;
        public bool IsSucceed => _isSucceed;
        //TODO: utilize correlation id for building robust distributed messages.
        public string CorrelationId { get; set; }
    }
}