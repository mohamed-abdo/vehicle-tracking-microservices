using System;

namespace Ping.Models
{
    [Serializable]
    public class PingRequest
    {
        public VehicleStatus Status { get; set; }
        public string Message { get; set; }
    }
}
