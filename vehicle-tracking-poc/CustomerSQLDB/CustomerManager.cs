using System;
using System.Linq;
using System.Threading.Tasks;
using DomainModels.System;
using Microsoft.Extensions.Logging;
using CustomerSQLDB.DbModels;

namespace CustomerSQLDB
{
    public class CustomerManager : ICommandCustomerDB<Customer>,
    IQueryCustomerDB<Customer>
    {
        public CustomerManager()
        {
        }
        private readonly CustomerDbContext _dbContext;
        private readonly ILogger _logger;
        public CustomerManager(ILoggerFactory loggerFactory, CustomerDbContext dbContext)
        {
            _logger = loggerFactory.CreateLogger(typeof(CustomerManager));
            _dbContext = dbContext;
        }
        public CustomerDbContext DbContext => _dbContext;
        public ILogger Logger => _logger;

        public Task<int> Add(Customer vehicle)
        {
            _dbContext.Customers.Add(vehicle);
            return _dbContext.SaveChangesAsync();
        }

        /// <summary>
        //design decision, max allowed returned rows are fixed by 1000
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<Customer> Query(Func<Customer, bool> predicate)
        {
            return _dbContext
                .Customers
                .Where(predicate)
                .Take(Identifiers.MaxRowsCount)
                .OrderBy(o=>o.Name)
                .AsQueryable();
        }
      
    }
}

