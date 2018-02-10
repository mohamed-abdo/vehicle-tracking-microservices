using Microsoft.AspNetCore.Mvc;
using Ping.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Ping.Contracts
{
    public interface IPing
    {
        Task<IActionResult> Post(string vehicleId, PingRequest pingRequest, CancellationToken cancellationToken);
    }
}
