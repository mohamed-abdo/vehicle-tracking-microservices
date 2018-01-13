using DomainModels.Types.Messages;
using EventSourceingSqlDb.DbModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourceingSqlDb.Repository
{
    public interface IEventSourceLedger<T> where T: new()
    {
        //command should be async task
        Task<int> Add((MessageHeader Header, T Body, MessageFooter Footer) pingEventSource);
        //query
        IQueryable<(MessageHeader Header, T Body, MessageFooter Footer)> Query(Func<(MessageHeader Header, T Body, MessageFooter Footer), bool> predicate);
    }
}
