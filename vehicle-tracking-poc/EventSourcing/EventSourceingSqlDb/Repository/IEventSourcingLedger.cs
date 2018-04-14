using DomainModels.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourceingSqlDb.Repository
{
    public interface ICommandEventSourcingLedger<T> where T : new()
    {
        //command should be async task
        Task<int> Add(T pingEventSourcing);
    }
    public interface IQueryEventSourcingLedger<T> where T : new()
    {
        // return type must be serializable, so i change from IQuerable
        IQueryable<T> Query(Func<T, bool> predicate);
        IQueryable<T> Query(IFilter queryFilter, Func<T, bool> predicate = null);
        IQueryable<T> Query(IFilter queryFilter, IDictionary<string,string> modelCriteria = null);
    }
}
