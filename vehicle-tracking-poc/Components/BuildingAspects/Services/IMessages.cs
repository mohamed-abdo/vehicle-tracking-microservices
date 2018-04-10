using DomainModels.Types;
using DomainModels.Types.Messages;
using System.Threading.Tasks;

namespace BuildingAspects.Services
{
    public interface IMessageCommand
    {
        Task Command<TResponse>(string exchange, string route, (MessageHeader Header, TResponse Body, MessageFooter Footer) message) where TResponse : DomainModel;
    }
    public interface IMessageQuery
    {
        Task<TRequset> Query<TRequset, TResponse>(string exchange, string route, (MessageHeader Header, TRequset Body, MessageFooter Footer) message) where TResponse : DomainModel;
    }
}
