using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Vehicle
{
    public class PingModel
    {
        public Guid VehicelId { get; set; }
        public Status Status { get; set; }
        public string StatusDescription { get; set; }
    }
}
