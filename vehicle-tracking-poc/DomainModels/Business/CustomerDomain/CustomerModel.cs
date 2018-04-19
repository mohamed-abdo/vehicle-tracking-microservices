using DomainModels.Business.VehicleDomain;
using DomainModels.Types;
using System;
using System.Collections.Generic;

namespace DomainModels.Business.CustomerDomain
{
    [Serializable]
    public class CustomerModel : DomainModel<Customer>
    {
        public CustomerModel() { }
        public CustomerModel(DomainModel<Customer> domainModel)
        {
            Header = domainModel.Header;
            Body = domainModel.Body;
            Footer = domainModel.Footer;
        }
    }
    [Serializable]
    public class Customer
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Country { get; set; }
        public virtual HashSet<Vehicle> Vehicles { get; set; }
    }
}
