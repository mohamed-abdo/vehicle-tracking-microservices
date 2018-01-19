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
    public class PingEventSourcingLedger : BaseEventSourcingLedger, IEventSourcingLedger<DbModel>
    {
        public PingEventSourcingLedger(ILoggerFactory loggerFactory, VehicleDbContext dbContext) : base(loggerFactory, dbContext) { }

        public Task<int> Add(DbModel pingEventSourcing)
        {
            DbContext.PingEventSource.Add(new PingEventSourcing(pingEventSourcing));
            return DbContext.SaveChangesAsync();
        }

        public IQueryable<DbModel> Query(Func<DbModel, bool> predicate)
        {
            return DbContext.PingEventSource.Where(predicate).AsQueryable();
        }
    }
}
