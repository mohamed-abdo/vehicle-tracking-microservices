using DomainModels.System;
using DomainModels.Types.Messages;
using DomainModels.Vehicle;
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
        Func<PingModel, (MessageHeader header, Ping body, MessageFooter footer)> Convert = (domainModel) =>
               (domainModel.Header, domainModel.Body, domainModel.Footer);

        private readonly PingEventSourcingLedger _pingEventSourcingLedger;
        public PingEventSourcingLedgerAdapter(ILoggerFactory loggerFactory, VehicleDbContext dbContext)
        {
            _pingEventSourcingLedger = new PingEventSourcingLedger(loggerFactory, dbContext);
        }
        public Task<int> Add(PingModel pingEventSourcing)
        {
            return
                _pingEventSourcingLedger
                .Add(Functors.Mappers<Ping>.FromPingModelToEnity(Convert(pingEventSourcing)));
        }

        public IQueryable<PingModel> Query(Func<PingModel, bool> predicate)
        {
            return
                _pingEventSourcingLedger
                .Query(Functors.Mappers<Ping>.PredicateMapper(Convert(predicate)))
                .Select(Functors.Mappers<Ping>.FromEnityToPingModel).AsQueryable();
        }
        public IQueryable<PingModel> Query(IFilter queryFilter, Func<PingModel, bool> predicate = null)
        {
            return
              _pingEventSourcingLedger
              .Query(queryFilter, Functors.Mappers<PingModel>.PredicateMapper(predicate))
              .Select(Functors.Mappers<PingModel>.FromEnityToPingModel).AsQueryable();
        }

        public IQueryable<PingModel> Query(IFilter queryFilter, IDictionary<string, string> modelCriteria = null)
        {
            throw new NotImplementedException();
        }
    }
}
