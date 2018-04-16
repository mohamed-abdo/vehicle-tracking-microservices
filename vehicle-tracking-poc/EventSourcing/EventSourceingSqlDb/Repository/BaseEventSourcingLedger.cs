using EventSourceingSQLDB.DbModels;
using Microsoft.Extensions.Logging;

namespace EventSourceingSQLDB.Repository
{
    public abstract class BaseEventSourcingLedger
    {
        private readonly EventSourcingDbContext _dbContext;
        private readonly ILogger _logger;
        public BaseEventSourcingLedger(ILoggerFactory loggerFactory, EventSourcingDbContext dbContext)
        {
            _logger = loggerFactory.CreateLogger(typeof(BaseEventSourcingLedger));
            _dbContext = dbContext;
        }
        public EventSourcingDbContext DbContext => _dbContext;
        public ILogger Logger => _logger;
    }
}
