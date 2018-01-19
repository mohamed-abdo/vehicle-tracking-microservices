using DomainModels.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Vehicle
{
    public class PingModel : IDescribe
    {
        public PingModel()
        {
            ReceivingTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }

        public readonly long ReceivingTimestamp;

        public string ChassisNumber { get; set; }
        public StatusModel Status { get; set; }
        public string Message { get; set; }

        public string Descripition => $"vehicle:{ChassisNumber},status:{Status},received:{ReceivingTimestamp}";
    }
}
