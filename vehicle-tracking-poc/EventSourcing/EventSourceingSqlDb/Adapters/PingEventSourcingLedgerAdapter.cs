using DomainModels.System;
using DomainModels.Types;
using DomainModels.Types.Messages;
using DomainModels.Business;
using EventSourceingSqlDb.DbModels;
using EventSourceingSqlDb.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourceingSqlDb.Adapters
{
    public class PingEventSourcingLedgerAdapter :
        ICommandEventSourcingLedger<PingModel>,
        IQueryEventSourcingLedger<PingModel>
    {
        private readonly PingEventSourcingLedger _pingEventSourcingLedger;

        Func<Func<PingModel, bool>, Func<DbModel, bool>> QueryConverter = (modelPredicate) =>
        {
            if (modelPredicate == null)
                return null;
            return (model) =>
            {
                return modelPredicate(new PingModel(DbModelFactory.Convert<Ping>(model)));
            };
        };
        public PingEventSourcingLedgerAdapter(ILoggerFactory loggerFactory, VehicleDbContext dbContext)
        {
            _pingEventSourcingLedger = new PingEventSourcingLedger(loggerFactory, dbContext);
        }
        public Task<int> Add(PingModel pingEventSourcing)
        {
            return
                _pingEventSourcingLedger
                .Add(DbModelFactory.Create(pingEventSourcing));
        }

        public IQueryable<PingModel> Query(Func<PingModel, bool> predicate)
        {
            return _pingEventSourcingLedger
                   .Query(QueryConverter(predicate))
                   .Select(q => new PingModel(DbModelFactory.Convert<Ping>(q)));
        }
        public IQueryable<PingModel> Query(IFilter queryFilter, Func<PingModel, bool> predicate = null)
        {
            return _pingEventSourcingLedger
                 .Query(queryFilter, QueryConverter(predicate))
                 .Select(q => new PingModel(DbModelFactory.Convert<Ping>(q)));
        }

        public IQueryable<PingModel> Query(IFilter queryFilter, IDictionary<string, string> modelCriteria = null)
        {
            throw new NotImplementedException();
        }
    }
}
