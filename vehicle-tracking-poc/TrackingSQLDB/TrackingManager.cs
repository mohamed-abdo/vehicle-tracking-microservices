using System;
using System.Linq;
using System.Threading.Tasks;
using DomainModels.System;
using Microsoft.Extensions.Logging;
using TrackingSQLDB.DbModels;

namespace TrackingSQLDB
{
    public class TrackingManager : ICommandTrackingDB<Tracking>,
    IQueryTrackingDB<Tracking>
    {
        public TrackingManager()
        {
        }
        private readonly TrackingDbContext _dbContext;
        private readonly ILogger _logger;
        public TrackingManager(ILoggerFactory loggerFactory, TrackingDbContext dbContext)
        {
            _logger = loggerFactory.CreateLogger(typeof(TrackingManager));
            _dbContext = dbContext;
        }
        public TrackingDbContext DbContext => _dbContext;
        public ILogger Logger => _logger;

        public Task<int> Add(Tracking vehicle)
        {
            _dbContext.Tracking.Add(vehicle);
            return _dbContext.SaveChangesAsync();
        }

        /// <summary>
        //design decision, max allowed returned rows are fixed by 1000
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<Tracking> Query(Func<Tracking, bool> predicate)
        {
            return _dbContext
                .Tracking
                .Where(predicate)
                .Take(Identifiers.MaxRowsCount)
                .OrderByDescending(o=>o.Timestamp)
                .AsQueryable();
        }
        /// <summary>
        //design decision, max allowed returned rows are fixed by 1000
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<Tracking> Query(IFilter queryFilter, Func<Tracking, bool> predicate = null)
        {
            if (predicate == null)
                predicate = (x) => true;
            Func<Tracking, bool> timeRangePredicate = (m) =>
            {
                return
                m.Timestamp >= queryFilter.StartFromTime &&
                m.Timestamp <= queryFilter.EndByTime;
            };
            return DbContext
                    .Tracking
                    .Where(predicate)
                    .Where(timeRangePredicate)
                    .Skip(queryFilter.PageNo * Identifiers.DataPageSize)
                    .Take(Math.Min(Identifiers.MaxRowsCount, queryFilter.rowsCount))
                    .OrderByDescending(o => o.Timestamp)
                    .AsQueryable();

        }
    }
}

