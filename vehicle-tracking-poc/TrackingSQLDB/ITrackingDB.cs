using DomainModels.System;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TrackingSQLDB
{
    public interface ICommandTrackingDB<T> where T : new()
    {
        //command should be async task
        Task<int> Add(T trackingDbModel);
    }
    public interface IQueryTrackingDB<T>
    {
        // return type must be serializable, so i change from IQuerable
        IQueryable<T> Query(Func<T, bool> predicate);
        IQueryable<T> Query(IFilter queryFilter, Func<T, bool> predicate = null);
    }

}
