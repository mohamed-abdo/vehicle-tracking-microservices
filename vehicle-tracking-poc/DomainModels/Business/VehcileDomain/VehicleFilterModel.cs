using DomainModels.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Business.VehicleDomain
{
    [Serializable]
    public class VehicleFilterModel : DomainModel<VehicleFilter>
    {
        public VehicleFilterModel() { }
        public VehicleFilterModel(DomainModel<VehicleFilter> domainModel)
        {
            Header = domainModel.Header;
            Body = domainModel.Body;
            Footer = domainModel.Footer;
        }
    }

    [Serializable]
    public class VehicleFilter
    {
        public Guid CorrelationId { get; set; }
        public Guid CustomerId { get; set; }
    }
}
