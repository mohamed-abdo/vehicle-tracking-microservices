using DomainModels.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Customer
{
    public class CustomerModel :IDescribe
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public DateTime Birthdate { get; set; }

        public string Nationality { get; set; }
        public string Address { get; set; }
        public string Notes { get; set; }

        public IList<(string ChassisNumber, string Model)> Vehicles { get; set; }
        public string Descripition => Name;
    }
}
