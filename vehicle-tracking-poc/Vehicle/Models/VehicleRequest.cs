using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vehicle.Models
{
    [Serializable]
    public class VehicleRequest
    {
        [Required]
        public string ChassisNumber { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        public string ProductionYear { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public Guid CustomerId { get; set; }

    }
}
