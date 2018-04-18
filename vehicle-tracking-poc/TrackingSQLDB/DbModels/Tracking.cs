using DomainModels.Business;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackingSQLDB.DbModels
{
    public class Tracking
    {
        public Tracking()
        {
        }
        public Tracking(DomainModels.Business.Tracking tracking)
        {
            ChassisNumber = tracking.ChassisNumber;
            Model = tracking.Model;
            CorrelationId = tracking.CorrelationId;
            Owner = tracking.Owner;
            OwnerRef = tracking.OwnerRef;
            Status = tracking.Status;
            Timestamp = tracking.Timestamp;
            Message = tracking.Message;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //Header
        public virtual long Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string ChassisNumber { get; set; }
        public string Model { get; set; }
        public string Owner { get; set; }
        public string OwnerRef { get; set; }
        public StatusModel Status { get; set; }
        public long Timestamp { get; set; }
        public string Message { get; set; }
    }
}
