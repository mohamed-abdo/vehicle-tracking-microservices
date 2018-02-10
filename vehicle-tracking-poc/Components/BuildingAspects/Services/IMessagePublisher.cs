using DomainModels.Types;
using DomainModels.Types.Messages;
using System.Threading.Tasks;

namespace BuildingAspects.Services
{
    public interface IMessagePublisher
    {
        Task Publish<T>(string exchange, string route, (MessageHeader Header, T Body, MessageFooter Footer) message) where T : DomainModel;
    }
}
