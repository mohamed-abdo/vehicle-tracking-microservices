using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.System
{
    public interface IDomainModel<T> where T : struct
    {
        Guid Id { get;}
        Guid CorrelateId { get; set; }
        T t { get; set; }
    }
}
