using EventSourceingSqlDb.DbModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventSourceingSqlDb.Repository
{
    public class PingEventSourceLedger : BaseEventSourceLedger, IEventSourceLedger<PingEventSource>
    {
        public PingEventSourceLedger(ILogger logger, VehicleDbContext dbContext) : base(logger, dbContext) { }
        public int Add(PingEventSource pingEventSource)
        {
            throw new NotImplementedException();
        }

        public IQueryable<PingEventSource> Query(Func<PingEventSource, bool> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
