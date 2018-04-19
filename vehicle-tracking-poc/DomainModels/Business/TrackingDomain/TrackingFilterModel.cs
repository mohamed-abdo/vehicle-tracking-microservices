using DomainModels.System;
using DomainModels.Types;
using DomainModels.Types.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Business.TrackingDomain
{
    [Serializable]
    public class TrackingFilterModel : DomainModel<TrackingFilter>
    {
        public TrackingFilterModel() { }
        public TrackingFilterModel(DomainModel<TrackingFilter> domainModel)
        {
            Header = domainModel.Header;
            Body = domainModel.Body;
            Footer = domainModel.Footer;
        }
    }
    [Serializable]
    public class TrackingFilter : IFilter
    {
        public long StartFromTime { get; set; }
        public long EndByTime { get; set; }
        public int rowsCount { get; set; }
        public int PageNo { get; set; }
    }
}
