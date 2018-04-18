using DomainModels.Business;
using System;

namespace Tracking.Models
{
    [Serializable]
    public class TrackingResponse
    {
        public TrackingResponse(DomainModels.Business.Tracking tracking)
        {
            CorrelationId = tracking.CorrelationId;
            ChassisNumber = tracking.ChassisNumber;
            Model = tracking.Model;
            Owner = tracking.Owner;
            OwnerRef = tracking.OwnerRef;
            Status = Enum.GetName(typeof(StatusModel), tracking.Status);
            Message = tracking.Message;
            Timestamp = tracking.Timestamp;
        }
        public Guid CorrelationId { get; set; }
        public string ChassisNumber { get; set; }
        public string Model { get; set; }
        public string Owner { get; set; }
        public string OwnerRef { get; set; }
        public string Status { get; set; }
        public DateTime UTCLastUpdate
        {
            get => DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).UtcDateTime;
        }
        public string Message { get; set; }
        private long Timestamp { get; set; }
    }
}
