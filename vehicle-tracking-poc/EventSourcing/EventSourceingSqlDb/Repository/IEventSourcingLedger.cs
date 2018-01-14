using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourceingSqlDb.Repository
{
    public interface IEventSourcingLedger<T> where T : new()
    {
        //command should be async task
        Task<int> Add(T pingEventSourcing);
        //query
        IQueryable<T> Query(Func<T, bool> predicate);
    }
}
