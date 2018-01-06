using DomainModels.Types;
using System;
using System.Collections;

namespace DomainModels.Vehicle
{
    public class VehicleModel : IDescribe
    {
        public string ChassisNumber { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string ProductionYear { get; set; }
        public string ProductoinFactory { get; set; }
        public IDictionary Features { get; set; }

        public string Descripition => $"model:{Model},color{Color},year:{ProductionYear},chassis:{ChassisNumber}";
    }
}
