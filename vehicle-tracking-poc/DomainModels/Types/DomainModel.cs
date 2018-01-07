using System;

namespace DomainModels.Types
{
    public sealed class DomainModel<T> where T : IDescribe, new()
    {
        public DomainModel()
        {
            InstanceId = Guid.NewGuid();
            ModelName = $"{default(T) as Type}";
            Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }
        public readonly long Timestamp;
        public readonly Guid InstanceId;
        public readonly string ModelName;

        public T Model { get; set; }
    }
}
