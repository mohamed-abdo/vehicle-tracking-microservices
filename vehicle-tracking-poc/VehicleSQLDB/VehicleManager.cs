using System;
using System.Linq;
using System.Threading.Tasks;
using DomainModels.System;
using Microsoft.Extensions.Logging;
using VehicleSQLDB.DbModels;

namespace VehicleSQLDB
{
    public class VehicleManager : ICommandVehicleDB<Vehicle>,
    IQueryVehicleDB<Vehicle>
    {
        public VehicleManager()
        {
        }
        private readonly VehicleDbContext _dbContext;
        private readonly ILogger _logger;
        public VehicleManager(ILoggerFactory loggerFactory, VehicleDbContext dbContext)
        {
            _logger = loggerFactory.CreateLogger(typeof(VehicleManager));
            _dbContext = dbContext;
        }
        public VehicleDbContext DbContext => _dbContext;
        public ILogger Logger => _logger;

        public Task<int> Add(Vehicle vehicle)
        {
            _dbContext.Vehicles.Add(vehicle);
            return _dbContext.SaveChangesAsync();
        }

        /// <summary>
        //design decision, max allowed returned rows are fixed by 1000
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<Vehicle> Query(Func<Vehicle, bool> predicate)
        {
            return _dbContext
                .Vehicles
                .Where(predicate)
                .Take(Identifiers.MaxRowsCount)
                .OrderBy(o=>o.ChassisNumber)
                .AsQueryable();
        }
      
    }
}

