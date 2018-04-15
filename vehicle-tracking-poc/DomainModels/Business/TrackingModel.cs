using DomainModels.Types;
using DomainModels.Types.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Business
{
    [Serializable]
    public class TrackingModel : DomainModel<Tracking>
    {
        public TrackingModel() { }
        public TrackingModel(DomainModel<Tracking> domainModel)
        {
            Header = domainModel.Header;
            Body = domainModel.Body;
            Footer = domainModel.Footer;
        }
    }
    [Serializable]
    public class Tracking
    {
        public string ChassisNumber { get; set; }
        public string Vehicle { get; set; }
        public string Owner { get; set; }
        public string OwnerRef { get; set; }
        public StatusModel Status { get; set; }
        public DateTime LastUpdate { get; set; }
        public string Message { get; set; }
    }
}
