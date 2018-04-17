using DomainModels.System;
using EventSourceingSQLDB.DbModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourceingSQLDB.Repository
{
    public class EventSourcingLedger : BaseEventSourcingLedger,
        ICommandEventSourcingLedger<DbModel>,
        IQueryEventSourcingLedger<DbModel>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="dbContext"></param>
        /// <param name="serviceFilter">default value null, query regardless sender service.</param>
        public EventSourcingLedger(ILoggerFactory loggerFactory, EventSourcingDbContext dbContext, string serviceFilter = null) : base(loggerFactory, dbContext)
        {
            _serviceFilter = serviceFilter;
        }
        private readonly string _serviceFilter;

        public Task<int> Add(DbModel pingEventSourcing)
        {
            DbContext.EventSourcing.Add(new EventSourcing(pingEventSourcing));
            return DbContext.SaveChangesAsync();
        }

        /// <summary>
        //design decision, max allowed returned rows are fixed by 1000
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<DbModel> Query(Func<DbModel, bool> predicate)
        {
            if (string.IsNullOrEmpty(_serviceFilter))
                return DbContext
                        .EventSourcing
                        .Where(predicate)
                        .Take(Identifiers.MaxRowsCount)
                        .OrderByDescending(o=>o.Timestamp)
                        .AsQueryable();
            else
                return DbContext
                    .EventSourcing
                    .Where(i => i.Sender.Equals(_serviceFilter, StringComparison.InvariantCultureIgnoreCase))
                    .Where(predicate)
                    .Take(Identifiers.MaxRowsCount)
                    .AsQueryable();
        }

        /// <summary>
        //design decision, max allowed returned rows are fixed by 1000
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<DbModel> Query(IFilter queryFilter, Func<DbModel, bool> predicate = null)
        {
            if (predicate == null)
                predicate = (x) => true;
            Func<DbModel, bool> timeRangePredicate = (m) =>
            {
                return
                m.Timestamp >= queryFilter.StartFromTime &&
                m.Timestamp <= queryFilter.EndByTime &&
                string.IsNullOrEmpty(_serviceFilter) ? true :
                m.Sender.Equals(_serviceFilter, StringComparison.InvariantCultureIgnoreCase);
            };

            return DbContext
                    .EventSourcing
                    .Where(predicate)
                    .Where(timeRangePredicate)
                    .Skip(queryFilter.PageNo * Identifiers.DataPageSize)
                    .Take(Math.Min(Identifiers.MaxRowsCount, queryFilter.rowsCount))
                    .OrderByDescending(o => o.Timestamp)
                    .AsQueryable();

        }

        public IQueryable<DbModel> Query(IFilter queryFilter, IDictionary<string, string> modelCriteria = null)
        {
            throw new NotImplementedException();
        }
    }
}
