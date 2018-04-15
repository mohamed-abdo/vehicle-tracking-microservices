using DomainModels.System;
using DomainModels.Types.Messages;
using DomainModels.Vehicle;
using EventSourceingSqlDb.DbModels;
using EventSourceingSqlDb.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourceingSqlDb.Adapters
{
    public class TrackingEventSourcingLedgerAdapter : IQueryEventSourcingLedger<TrackingModel>
    {

        private readonly PingEventSourcingLedger _pingEventSourcingLedger;

        Func<Func<TrackingModel, bool>, Func<DbModel, bool>> QueryConverter = (modelPredicate) =>
        {
            return (model) =>
            {
                return modelPredicate(new TrackingModel(DbModelFactory.Convert<Tracking>(model)));
            };
        };

        public TrackingEventSourcingLedgerAdapter(ILoggerFactory loggerFactory, VehicleDbContext dbContext)
        {
            _pingEventSourcingLedger = new PingEventSourcingLedger(loggerFactory, dbContext);
        }

        public IQueryable<TrackingModel> Query(Func<TrackingModel, bool> predicate)
        {
            return _pingEventSourcingLedger
                     .Query(QueryConverter(predicate))
                     .Select(q => new TrackingModel(DbModelFactory.Convert<Tracking>(q)));
        }
        public IQueryable<TrackingModel> Query(IFilter queryFilter, Func<TrackingModel, bool> predicate = null)
        {
            return _pingEventSourcingLedger
                   .Query(queryFilter, QueryConverter(predicate))
                   .Select(q => new TrackingModel(DbModelFactory.Convert<Tracking>(q)));
        }

        public IQueryable<TrackingModel> Query(IFilter queryFilter, IDictionary<string, string> modelCriteria = null)
        {
            throw new NotImplementedException();
        }
    }
}
