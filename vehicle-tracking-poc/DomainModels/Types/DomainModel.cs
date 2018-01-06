using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Types
{
    public sealed class DomainModel<T> where T : IDescribe
    {
        public DomainModel()
        {
            InstanceId = Guid.NewGuid();
            ModelName = nameof(T);
            Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }
        public readonly long Timestamp;
        public readonly Guid InstanceId;
        public readonly string ModelName;

        public T Model { get; set; }
    }
}
