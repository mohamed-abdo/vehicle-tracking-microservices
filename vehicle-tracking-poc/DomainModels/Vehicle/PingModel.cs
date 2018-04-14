using DomainModels.Types;
using System;

namespace DomainModels.Vehicle
{
    [Serializable]
    public class PingModel : DomainModel<Ping>
    {
    }
    [Serializable]
    public class Ping
    {
        public string ChassisNumber { get; set; }
        public StatusModel Status { get; set; }
        public string Message { get; set; }
    }
}
