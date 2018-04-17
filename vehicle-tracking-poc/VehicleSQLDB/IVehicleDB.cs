using System;
using System.Linq;
using System.Threading.Tasks;

namespace VehicleSQLDB
{

    public interface ICommandVehicleDB<T> where T : new()
    {
        //command should be async task
        Task<int> Add(T vehicleDbModel);
    }
    public interface IQueryVehicleDB<T>
    {
        // return type must be serializable, so i change from IQuerable
        IQueryable<T> Query(Func<T, bool> predicate);
    }

}
