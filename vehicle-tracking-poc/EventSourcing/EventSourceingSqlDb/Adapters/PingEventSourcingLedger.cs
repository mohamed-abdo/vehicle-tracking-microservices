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
    public class PingEventSourcingLedger : IEventSourcingLedger<(MessageHeader header, PingModel body, MessageFooter footer)>
    {
        private readonly Repository.PingEventSourceLedger _pingEventSourcingLedger;
        public PingEventSourcingLedger(ILoggerFactory loggerFactory, VehicleDbContext dbContext)
        {
            _pingEventSourcingLedger = new Repository.PingEventSourceLedger(loggerFactory, dbContext);
        }
        public Task<int> Add((MessageHeader header, PingModel body, MessageFooter footer) pingEventSourcing)
        {
            return 
                _pingEventSourcingLedger
                .Add(Functors.Mappers<PingModel>.FromPingModelToEnity(pingEventSourcing));
        }

        public IQueryable<(MessageHeader header, PingModel body, MessageFooter footer)> Query(Func<(MessageHeader header, PingModel body, MessageFooter footer), bool> predicate)
        {
            return
                _pingEventSourcingLedger
                .Query(Functors.Mappers<PingModel>.PredicateMapper(predicate))
                .Select(Functors.Mappers<PingModel>.FromEnityToPingModel)
                .AsQueryable();
        }
    }
}
