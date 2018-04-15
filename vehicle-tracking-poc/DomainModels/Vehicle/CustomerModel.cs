using DomainModels.Types;
using DomainModels.Types.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Vehicle
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
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public DateTime BirthDate { get; set; }
        public string Country { get; set; }
    }
}
