using DomainModels.System;
using DomainModels.Business;
using EventSourceingSQLDB.DbModels;
using EventSourceingSQLDB.Repository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using DomainModels.Business.TrackingDomain;

namespace EventSourceingSQLDB.Adapters
{
    public class TrackingEventSourcingLedgerAdapter : IQueryEventSourcingLedger<TrackingModel>
    {

        private readonly EventSourcingLedger _pingEventSourcingLedger;

        Func<Func<TrackingModel, bool>, Func<DbModel, bool>> QueryConverter = (modelPredicate) =>
        {
            if (modelPredicate == null)
                return null;
            return (model) =>
            {
                return modelPredicate(new TrackingModel(DbModelFactory.Convert<Tracking>(model)));
            };
        };

        public TrackingEventSourcingLedgerAdapter(ILoggerFactory loggerFactory, EventSourcingDbContext dbContext)
        {
            _pingEventSourcingLedger = new EventSourcingLedger(loggerFactory, dbContext, Identifiers.TrackingServiceName);
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
