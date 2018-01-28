using System;
using System.Runtime.Serialization;

namespace DomainModels.Types
{
    public interface IDomainModel<T> where T : IDescribe, ISerializable, new()
    {
     
    }
}
