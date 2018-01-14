using EventSourceingSqlDb.DbModels;
using Microsoft.Extensions.Logging;

namespace EventSourceingSqlDb.Repository
{
    public abstract class BaseEventSourceLedger
    {
        private readonly VehicleDbContext _dbContext;
        private readonly ILogger _logger;
        public BaseEventSourceLedger(ILoggerFactory loggerFactory, VehicleDbContext dbContext)
        {
            _logger = loggerFactory.CreateLogger(typeof(BaseEventSourceLedger));
            _dbContext = dbContext;
        }
        public VehicleDbContext DbContext => _dbContext;
        public ILogger Logger => _logger;
    }
}
