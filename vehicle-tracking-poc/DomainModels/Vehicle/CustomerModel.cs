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
