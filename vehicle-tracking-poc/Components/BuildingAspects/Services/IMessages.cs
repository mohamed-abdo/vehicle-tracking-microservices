using DomainModels.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildingAspects.Services
{
    public interface IMessageCommand
    {
        Task Command<TRequest>(TRequest message);
    }

    public interface IMessageRequest<TRequest, TResponse>
    {
        Task<TResponse> Request(TRequest message);
    }
}
