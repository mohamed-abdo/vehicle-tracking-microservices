using DomainModels.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vehicleStatus.Ping
{
    public interface IPingREST
    {
       IResponseModel Ping(string vehicleId);
    }
}
