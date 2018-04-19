using System;
using System.Collections.Generic;

namespace Customer.Models
{
    [Serializable]
    public class VehicleResponse
    {
        public VehicleResponse(DomainModels.Business.VehicleDomain.Vehicle vehicle)
        {
            CorrelationId = vehicle.CorrelationId;
            ChassisNumber = vehicle.ChassisNumber;
            Model = vehicle.Model;
            Color = vehicle.Color;
            Country = vehicle.Country;
            ProductionYear = vehicle.ProductionYear;
            Features = vehicle.Features;
        }
        public Guid CorrelationId { get; set; }
        public string ChassisNumber { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string ProductionYear { get; set; }
        public string Country { get; set; }
        public HashSet<string> Features { get; set; }
    }
}
