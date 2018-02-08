using Ping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ping.Contracts
{
    public interface IPing
    {
        Task<IAsyncResult> Post(string vehicleId, PingRequest pingRequest, CancellationToken cancellationToken);
    }
}
