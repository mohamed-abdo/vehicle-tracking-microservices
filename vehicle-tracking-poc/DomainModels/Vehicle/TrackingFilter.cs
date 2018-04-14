using DomainModels.System;
using DomainModels.Types;
using DomainModels.Types.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels.Vehicle
{
    [Serializable]
    public class TrackingFilterModel : DomainModel<TrackingFilter>
    {
    }
    [Serializable]
    public class TrackingFilter : IFilter
    {
        public long StartFromTime { get; set; }
        public long EndByTime { get; set; }
        public int PageSize { get; set; }
        public int PageNo { get; set; }
    }
}
