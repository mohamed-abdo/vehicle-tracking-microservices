using DomainModels.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ping.Models
{
    public class PingRequest
    {
        public Guid VehicelId { get; set; }
        public string Name { get; set; }
        public string StatusDescription { get; set; }
    }
}
