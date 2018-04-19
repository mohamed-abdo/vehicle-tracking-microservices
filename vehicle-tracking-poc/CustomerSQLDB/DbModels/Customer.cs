using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerSQLDB.DbModels
{
    public class Customer
    {
        public Customer()
        {
        }
        public Customer(DomainModels.Business.CustomerDomain.Customer customer)
        {
            Id = customer.Id;
            Name = customer.Name;
            CorrelationId = customer.CorrelationId;
            Country = customer.Country;
            Mobile = customer.Mobile;
            Email = customer.Email;
            BirthDate = customer.BirthDate;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //Header
        public virtual Guid Id { get; set; }
        public virtual Guid CorrelationId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string Email { get; set; }
        public virtual DateTime? BirthDate { get; set; }
        public virtual string Country { get; set; }
    }
}
