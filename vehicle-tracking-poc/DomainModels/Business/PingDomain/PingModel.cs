using DomainModels.Types;
using DomainModels.Types.Messages;
using System;

namespace DomainModels.Business.PingDomain
{
    [Serializable]
    public class PingModel : DomainModel<Ping>
    {
        public PingModel() { }
        public PingModel(DomainModel<Ping> domainModel)
        {
            Header = domainModel.Header;
            Body = domainModel.Body;
            Footer = domainModel.Footer;
        }
    }
    [Serializable]
    public class Ping
    {
        public string ChassisNumber { get; set; }
        public StatusModel Status { get; set; }
        public string Message { get; set; }
    }
}
