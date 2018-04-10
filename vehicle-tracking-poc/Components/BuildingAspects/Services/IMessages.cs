using DomainModels.Types;
using DomainModels.Types.Messages;
using System.Threading.Tasks;

namespace BuildingAspects.Services
{
    public interface IMessageCommand
    {
        Task Command<TResponse>(string exchange, string route, (MessageHeader Header, TResponse Body, MessageFooter Footer) message);
    }
    public interface IMessageQuery<TRequset, TResponse>
    {
        Task<TResponse> Query(string exchange, string route, (MessageHeader Header, TRequset Body, MessageFooter Footer) message);
    }
}
