using DomainModels.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DomainModels.Business.VehicleDomain
{
    [Serializable]
    public class VehicleModel : DomainModel<Vehicle>
    {
        public VehicleModel() { }
        public VehicleModel(DomainModel<Vehicle> domainModel)
        {
            Header = domainModel.Header;
            Body = domainModel.Body;
            Footer = domainModel.Footer;
        }
    }
    [Serializable]
    public class Vehicle
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string ChassisNumber { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string ProductionYear { get; set; }
        public string Country { get; set; }
        public HashSet<string> Features { get; set; }

        // references (following eventually consistence)
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }

    }
}
