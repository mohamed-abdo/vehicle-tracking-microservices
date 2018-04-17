using System;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerSQLDB
{

    public interface ICommandCustomerDB<T> where T : new()
    {
        //command should be async task
        Task<int> Add(T vehicleDbModel);
    }
    public interface IQueryCustomerDB<T>
    {
        // return type must be serializable, so i change from IQuerable
        IQueryable<T> Query(Func<T, bool> predicate);
    }

}
