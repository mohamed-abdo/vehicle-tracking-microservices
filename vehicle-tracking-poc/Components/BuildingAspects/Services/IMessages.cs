using DomainModels.Types;
using DomainModels.Types.Messages;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BuildingAspects.Services
{
    public interface IMessageCommand
    {
        Task Command<TRequest>((MessageHeader Header, TRequest Body, MessageFooter Footer) message);
    }

    public interface IMessageQuery<TRequset, TResponse>
    {
        Task<TResponse> Query((MessageHeader Header, TRequset Body, MessageFooter Footer) message);
    }
}
