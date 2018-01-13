using EventSourceingSqlDb.DbModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventSourceingSqlDb.Repository
{
    public interface IEventSourceLedger<T> where T: BaseModel
    {
        int Add(T pingEventSource);
        IQueryable<T> Query(Func<T, bool> predicate);
    }
}
