using DomainModels.Types.Messages;
using System.Threading.Tasks;

namespace BackgroundMiddleware.Abstract
{
    public interface IMessagePublisher
    {
        Task Publish<T>(string exchange, string route, (MessageHeader Header, T Body, MessageFooter Footer) message);
    }
}
