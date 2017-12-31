using DomainModels.System;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundMiddleware.Abstract
{
    public interface IMessagePublisher
    {
        Task Publish<T>(string exchange, string route, (MessageHeader Header, T Body, MessageFooter Footer) message);
    }
}
