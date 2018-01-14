using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourceingSqlDb.Repository
{
    public interface IEventSourceLedger<T> where T : new()
    {
        //command should be async task
        Task<int> Add(T pingEventSource);
        //query
        IQueryable<T> Query(Func<T, bool> predicate);
    }
}
