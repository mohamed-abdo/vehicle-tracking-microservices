using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VehicleSQLDB.DbModels
{
    public class Vehicle
    {
        public Vehicle()
        {
        }
        public Vehicle(DomainModels.Business.Vehicle vehicle)
        {
            Id = vehicle.Id;
            CorrelationId = vehicle.CorrelationId;
            ChassisNumber = vehicle.ChassisNumber;
            Model = vehicle.Model;
            Color = vehicle.Color;
            ProductionYear = vehicle.ProductionYear;
            Country = vehicle.Country;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //Header
        public virtual Guid Id { get; set; }
        public virtual string CorrelationId { get; set; }
        public virtual string ChassisNumber { get; set; }
        public virtual string Model { get; set; }
        public virtual string Color { get; set; }
        public virtual string ProductionYear { get; set; }
        public virtual string Country { get; set; }
    }
}
