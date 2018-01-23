using DomainModels.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Vehicle
{
    [Serializable]
    public class TrackingModel : DomainModel
    {
        public string ChassisNumber { get; set; }
        public string Vehicle { get; set; }
        public string Owner { get; set; }
        public string OwnerRef { get; set; }
        public StatusModel Status { get; set; }
        public DateTime LastUpdate { get; set; }
        public string Message { get; set; }

        public override string Descripition => $"vehicle:{ChassisNumber},status:{Status},last-update:{LastUpdate}";
    }
}
