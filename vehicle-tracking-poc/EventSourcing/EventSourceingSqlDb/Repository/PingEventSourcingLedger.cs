using DomainModels.System;
using EventSourceingSqlDb.DbModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourceingSqlDb.Repository
{
    public class PingEventSourcingLedger : BaseEventSourcingLedger,
        ICommandEventSourcingLedger<DbModel>,
        IQueryEventSourcingLedger<DbModel>
    {
        public PingEventSourcingLedger(ILoggerFactory loggerFactory, VehicleDbContext dbContext) : base(loggerFactory, dbContext) { }

        public Task<int> Add(DbModel pingEventSourcing)
        {
            DbContext.PingEventSource.Add(new PingEventSourcing(pingEventSourcing));
            return DbContext.SaveChangesAsync();
        }

        public IQueryable<DbModel> Query(Func<DbModel, bool> predicate)
        {
            return DbContext
                    .PingEventSource
                    .Where(predicate)
                    .AsQueryable();
        }
        public IQueryable<DbModel> Query(IFilter queryFilter, Func<DbModel, bool> predicate = null)
        {
            if (predicate == null)
                predicate = (x) => true;
            Func<DbModel, bool> timeRangePredicate = (m) =>
            {
                return
                m.Timestamp >= queryFilter.StartFromTime &&
                m.Timestamp <= queryFilter.EndByTime;
            };

            return DbContext
                    .PingEventSource
                    .Where(predicate)
                    .Where(timeRangePredicate)
                    .Skip(queryFilter.PageNo * Identifiers.DataPageSize)
                    .Take(queryFilter.rowsCount)
                    .OrderByDescending(o => o.Timestamp)
                    .AsQueryable();
        }

        public IQueryable<DbModel> Query(IFilter queryFilter, IDictionary<string, string> modelCriteria = null)
        {
            throw new NotImplementedException();
        }
    }
}
