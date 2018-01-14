using DomainModels.Types.Messages;
using DomainModels.Vehicle;
using EventSourceingSqlDb.DbModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourceingSqlDb.Repository
{
    public class PingEventSourceLedger : BaseEventSourceLedger, IEventSourceLedger<PingEventSourcing>
    {
        public PingEventSourceLedger(ILogger logger, VehicleDbContext dbContext) : base(logger, dbContext) { }

        public Task<int> Add((MessageHeader Header, PingModel Body, MessageFooter Footer) pingEventSource)
        {
            DbContext.PingEventSource.Add(Functors.Mappers<PingModel, PingEventSourcing>.FromPingModelToEnity(pingEventSource));
            return DbContext.SaveChangesAsync();
        }

        public Task<int> Add(PingEventSourcing pingEventSource)
        {
            DbContext.PingEventSource.Add(pingEventSource);
            return DbContext.SaveChangesAsync();
        }

        public IQueryable<PingEventSourcing> Query(Func<PingEventSourcing, bool> predicate)
        {
            return DbContext.PingEventSource.Where(predicate).AsQueryable();
        }
    }
}
