using DomainModels.System;
using DomainModels.Types.Messages;
using DomainModels.Business;
using EventSourceingSQLDB.DbModels;
using EventSourceingSQLDB.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventSourceingSQLDB.Adapters
{
    public class TrackingEventSourcingLedgerAdapter : IQueryEventSourcingLedger<TrackingModel>
    {

        private readonly PingEventSourcingLedger _pingEventSourcingLedger;

        Func<Func<TrackingModel, bool>, Func<DbModel, bool>> QueryConverter = (modelPredicate) =>
        {
            if (modelPredicate == null)
                return null;
            return (model) =>
            {
                return modelPredicate(new TrackingModel(DbModelFactory.Convert<Tracking>(model)));
            };
        };

        public TrackingEventSourcingLedgerAdapter(ILoggerFactory loggerFactory, VehicleDbContext dbContext)
        {
            _pingEventSourcingLedger = new PingEventSourcingLedger(loggerFactory, Identifiers.TrackingServiceName, dbContext);
        }

        public string Sender => Identifiers.TrackingServiceName;

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
