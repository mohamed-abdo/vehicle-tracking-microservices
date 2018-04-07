using DomainModels.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Vehicle
{
    [Serializable]
    public class PingModel : DomainModel
    {
        public PingModel()
        {
            ReceivingTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }

        public readonly long ReceivingTimestamp;

        public string ChassisNumber { get; set; }
        public StatusModel Status { get; set; }
        public string Message { get; set; }

        public override string Description => $"vehicle:{ChassisNumber},status:{Status},received:{ReceivingTimestamp}";
    }
}
