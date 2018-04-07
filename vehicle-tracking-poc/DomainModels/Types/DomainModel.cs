using System;
using System.Runtime.Serialization;

namespace DomainModels.Types
{
    [Serializable]
    public abstract class DomainModel : IDescribe
    {
        public abstract string Description { get; }
    }
}
