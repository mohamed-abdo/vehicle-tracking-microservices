using EventSourceingSqlDb.DbModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventSourceingSqlDb.Repository
{
    public abstract class BaseEventSourceLedger 
    {
        private readonly VehicleDbContext _dbContext;
        private readonly ILogger _logger;
        public BaseEventSourceLedger(ILogger logger, VehicleDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        public VehicleDbContext DbContext => _dbContext;
        public ILogger Logger => _logger;
    }
}
