using BuildingAspects.Behaviors;
using DomainModels.Types.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventSourceingSqlDb.DbModels
{
    public static class DbModelFactory
    {
        public static DbModel Create<T>(MessageHeader header, T body, MessageFooter footer)
        {
            return new DbModel
            {
                ExecutionId = header.ExecutionId,
                CorrelationId = header.CorrelationId,
                Timestamp = header.Timestamp,

                Data = JObject.FromObject(body),

                Sender = footer.Sender,
                Environment = footer.Environment,
                Assembly = footer.Assembly,
                FingerPrint = footer.FingerPrint,
                Hint = Enum.GetName(typeof(ResponseHint), footer.Hint),
                Route = JsonConvert.DeserializeObject<IDictionary<string, string>>(footer.Route, Defaults.JsonSerializerSettings)
            };
        }

    }
}
