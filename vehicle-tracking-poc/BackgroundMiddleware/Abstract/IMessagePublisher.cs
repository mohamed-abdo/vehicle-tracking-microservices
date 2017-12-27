using DomainModels.System;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundMiddleware.Abstract
{
    public interface IMessagePublisher
    {
        void Publish<T>(string exchange, string route, T t) where T : struct, IDomainModel<T>;
    }
}
