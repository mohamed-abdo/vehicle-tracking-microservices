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
    public class TrackingEventSourcingLedgerAdapter : IQueryEventSourcingLedger<(MessageHeader header, TrackingModel body, MessageFooter footer)>
    {
        private readonly PingEventSourcingLedger _pingEventSourcingLedger;
        public TrackingEventSourcingLedgerAdapter(ILoggerFactory loggerFactory, VehicleDbContext dbContext)
        {
            _pingEventSourcingLedger = new PingEventSourcingLedger(loggerFactory, dbContext);
        }
      
        public IEnumerable<(MessageHeader header, TrackingModel body, MessageFooter footer)> Query(Func<(MessageHeader header, TrackingModel body, MessageFooter footer), bool> predicate)
        {
            return
                _pingEventSourcingLedger
                .Query(Functors.Mappers<TrackingModel>.PredicateMapper(predicate))
                .Select(Functors.Mappers<TrackingModel>.FromEnityToPingModel)
                .AsQueryable();
        }
    }
}
