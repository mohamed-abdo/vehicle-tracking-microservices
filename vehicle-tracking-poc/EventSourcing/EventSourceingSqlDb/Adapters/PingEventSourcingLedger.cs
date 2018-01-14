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
    public class PingEventSourcingLedger : IEventSourceLedger<(MessageHeader header, PingModel body, MessageFooter footer)>
    {
        private readonly Repository.PingEventSourceLedger _pingEventSourcingLedger;
        public PingEventSourcingLedger(ILogger logger, VehicleDbContext dbContext)
        {
            _pingEventSourcingLedger = new Repository.PingEventSourceLedger(logger, dbContext);
        }
        public Task<int> Add((MessageHeader header, PingModel body, MessageFooter footer) pingEventSource)
        {
            return 
                _pingEventSourcingLedger
                .Add(Functors.Mappers<PingModel, PingEventSourcing>.FromPingModelToEnity(pingEventSource));
        }

        public IQueryable<(MessageHeader header, PingModel body, MessageFooter footer)> Query(Func<(MessageHeader header, PingModel body, MessageFooter footer), bool> predicate)
        {
            return
                _pingEventSourcingLedger
                .Query(Functors.Mappers<PingModel, PingEventSourcing>.PredicateMapper(predicate))
                .Select(Functors.Mappers<PingModel, PingEventSourcing>.FromEnityToPingModel)
                .AsQueryable();
        }
    }
}
