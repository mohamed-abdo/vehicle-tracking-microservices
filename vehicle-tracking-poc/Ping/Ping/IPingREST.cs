using DomainModels.Types;

namespace vehicleStatus.Ping
{
    public interface IPingREST
    {
       ResponseModel Ping(string vehicleId);
    }
}
