using EventSourceingSqlDb.DbModels;
using Microsoft.Extensions.Logging;

namespace EventSourceingSqlDb.Repository
{
    public abstract class BaseEventSourcingLedger
    {
        private readonly VehicleDbContext _dbContext;
        private readonly ILogger _logger;
        public BaseEventSourcingLedger(ILoggerFactory loggerFactory, VehicleDbContext dbContext)
        {
            _logger = loggerFactory.CreateLogger(typeof(BaseEventSourcingLedger));
            _dbContext = dbContext;
        }
        public VehicleDbContext DbContext => _dbContext;
        public ILogger Logger => _logger;
    }
}
