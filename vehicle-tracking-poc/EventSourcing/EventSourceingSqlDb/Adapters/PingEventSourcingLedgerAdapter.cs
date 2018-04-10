using DomainModels.Types.Messages;
using DomainModels.Vehicle;
using EventSourceingSqlDb.DbModels;
using EventSourceingSqlDb.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourceingSqlDb.Adapters
{
    public class PingEventSourcingLedgerAdapter : ICommandEventSourcingLedger<(MessageHeader header, PingModel body, MessageFooter footer)>
        , IQueryEventSourcingLedger<(MessageHeader header, PingModel body, MessageFooter footer)>
    {
        private readonly PingEventSourcingLedger _pingEventSourcingLedger;
        public PingEventSourcingLedgerAdapter(ILoggerFactory loggerFactory, VehicleDbContext dbContext)
        {
            _pingEventSourcingLedger = new PingEventSourcingLedger(loggerFactory, dbContext);
        }
        public Task<int> Add((MessageHeader header, PingModel body, MessageFooter footer) pingEventSourcing)
        {
            return
                _pingEventSourcingLedger
                .Add(Functors.Mappers<PingModel>.FromPingModelToEnity(pingEventSourcing));
        }

        public IEnumerable<(MessageHeader header, PingModel body, MessageFooter footer)> Query(Func<(MessageHeader header, PingModel body, MessageFooter footer), bool> predicate)
        {
            return
                _pingEventSourcingLedger
                .Query(Functors.Mappers<PingModel>.PredicateMapper(predicate))
                .Select(Functors.Mappers<PingModel>.FromEnityToPingModel)
                .AsQueryable();
        }
    }
}
