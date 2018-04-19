using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleSQLDB.DbModels
{
    public class Vehicle
    {
        public Vehicle()
        {
        }
        public Vehicle(DomainModels.Business.VehicleDomain.Vehicle vehicle)
        {
            Id = vehicle.Id;
            CorrelationId = vehicle.CorrelationId;
            ChassisNumber = vehicle.ChassisNumber;
            Model = vehicle.Model;
            Color = vehicle.Color;
            ProductionYear = vehicle.ProductionYear;
            Country = vehicle.Country;
            CustomerId = vehicle.CustomerId;
            CustomerName = vehicle.CustomerName;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //Header
        public virtual Guid Id { get; set; }
        public virtual Guid CorrelationId { get; set; }
        public virtual string ChassisNumber { get; set; }
        public virtual string Model { get; set; }
        public virtual string Color { get; set; }
        public virtual string ProductionYear { get; set; }
        public virtual string Country { get; set; }

        // references (following eventually consistence)
        public virtual Guid CustomerId { get; set; }
        public virtual string CustomerName { get; set; }
    }
}
