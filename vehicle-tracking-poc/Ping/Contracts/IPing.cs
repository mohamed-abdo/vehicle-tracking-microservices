using Ping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ping.Contracts
{
    public interface IPing
    {
        IAsyncResult Post(string vehicleId, PingRequest pingRequest);
    }
}
