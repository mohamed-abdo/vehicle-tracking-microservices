using System;

namespace DomainModels.Types
{
    public sealed class DomainModel<T> where T : IDescribe, new()
    {
        public DomainModel()
        {
            ModelName = $"{default(T) as Type}";
        }
        public readonly string ModelName;

        public T Model { get; set; }
    }
}
