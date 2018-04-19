using DomainModels.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Business.CustomerDomain
{
    [Serializable]
    public class CustomerFilterModel : DomainModel<CustomerFilter>
    {
        public CustomerFilterModel() { }
        public CustomerFilterModel(DomainModel<CustomerFilter> domainModel)
        {
            Header = domainModel.Header;
            Body = domainModel.Body;
            Footer = domainModel.Footer;
        }
    }

    [Serializable]
    public class CustomerFilter
    {
        public Guid CorrelationId { get; set; }
    }
}
