using BuildingAspects.Behaviors;
using DomainModels.Types;
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
                Hint = footer.Hint,
                Route = JsonConvert.DeserializeObject<IDictionary<string, string>>(footer.Route, Defaults.JsonSerializerSettings)
            };
        }
        public static DbModel Create<T>(DomainModel<T> message)
        {
            return Create(message.Header, message.Body, message.Footer);
        }

        public static DomainModel<T> Convert<T>(DbModel model)
        {
            var header = new MessageHeader(executionId: model.ExecutionId, timestamp: model.Timestamp, isSucceed: true);
            var body = model.Data.ToObject<T>();
            var footer = new MessageFooter
            {
                Assembly = model.Assembly,
                Environment = model.Environment,
                FingerPrint = model.FingerPrint,
                Hint = model.Hint,
                Route = JsonConvert.SerializeObject(model.Route, Defaults.JsonSerializerSettings),
                Sender = model.Sender
            };
            return new DomainModel<T> { Header = header, Body = body, Footer = footer };
        }
    }

}

