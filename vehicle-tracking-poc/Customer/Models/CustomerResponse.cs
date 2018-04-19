using System;
using System.Collections.Generic;
using System.Linq;

namespace Customer.Models
{
    [Serializable]
    public class CustomerResponse
    {
        public CustomerResponse(DomainModels.Business.CustomerDomain.Customer customer)
        {
            CustomerId = customer.Id;
            CorrelationId = customer.CorrelationId;
            Name= customer.Name;
            Mobile = customer.Mobile;
            Email = customer.Email;
            BirthDate = customer.BirthDate;
            Country = customer.Country;
            Vehicles = customer.Vehicles?.Select(v => new VehicleResponse(v));
        }
        public Guid CustomerId { get; set; }
        public Guid CorrelationId { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Country { get; set; }
        public virtual IEnumerable<VehicleResponse> Vehicles { get; set; }
    }

}
