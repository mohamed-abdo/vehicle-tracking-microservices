using System;

namespace Ping.Models
{
    public class PingRequest
    {
        public Guid VehicelId { get; set; }
        public string Name { get; set; }
        public string StatusDescription { get; set; }
    }
}
