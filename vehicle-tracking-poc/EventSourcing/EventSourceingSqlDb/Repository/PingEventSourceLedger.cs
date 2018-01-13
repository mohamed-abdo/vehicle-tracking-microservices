using DomainModels.Types.Messages;
using DomainModels.Vehicle;
using EventSourceingSqlDb.DbModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourceingSqlDb.Repository
{
    public class PingEventSourceLedger : BaseEventSourceLedger, IEventSourceLedger<PingModel>
    {
        public PingEventSourceLedger(ILogger logger, VehicleDbContext dbContext) : base(logger, dbContext) { }

        public Task<int> Add((MessageHeader Header, PingModel Body, MessageFooter Footer) pingEventSource)
        {
            var entityObj = DbContext.PingEventSource.Add(Functors.Mappers.FromPingModelToEnity(pingEventSource));
            return DbContext.SaveChangesAsync();
        }

        public IQueryable<(MessageHeader Header, PingModel Body, MessageFooter Footer)> Query(Func<(MessageHeader Header, PingModel Body, MessageFooter Footer), bool> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
