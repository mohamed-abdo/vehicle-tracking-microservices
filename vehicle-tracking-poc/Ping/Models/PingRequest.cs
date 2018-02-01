using System;

namespace Ping.Models
{
    public struct PingRequest
    {
        public VehicleStatus Status { get; set; }
        public string Description { get; set; }
    }
}
