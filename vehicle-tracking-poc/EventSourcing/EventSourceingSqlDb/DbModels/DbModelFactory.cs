using DomainModels.Types.Messages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventSourceingSqlDb.DbModels
{
    public static class DbModelFactory
    {
        public static DbModel Create<T>(MessageHeader header,T body, MessageFooter footer)
        {
            return new DbModel
            {
                ExecutionId = header.ExecutionId,
                CorrelateId = header.CorrelateId,
                Timestamp = header.Timestamp,

                Data = JObject.FromObject(body),

                Sender = footer.Sender,
                Route = footer.Route,
                Environment = footer.Environment,
                Assembly = footer.Assembly,
                FingerPrint = footer.FingerPrint,
                Hint = footer.Hint,
            };
        }
    }
}
