using DomainModels.Types;
using DomainModels.Types.Messages;
using DomainModels.Vehicle;
using EventSourceingSqlDb.DbModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventSourceingSqlDb.Functors
{
    public static class Mappers
    {
        public static Action<MessageHeader, BaseDbModel> fillHeader = (header, model) =>
        {
            model.ExecutionId = header.ExecutionId;
            model.CorrelateId = header.CorrelateId;
            model.Timestamp = header.Timestamp;
        };

        public static Action<MessageFooter, BaseDbModel> fillFooter = (footer, model) =>
        {
            model.Sender = footer.Sender;
            model.Route = footer.Route;
            model.Environemnt = footer.Environemnt;
            model.Assembly = footer.Assembly;
            model.FingerPrint = footer.FingerPrint;
            model.Hint = footer.Hint;
        };

        public static Func<(MessageHeader Header, PingModel Body, MessageFooter Footer), PingEventSource> FromPingModelToEnity = (pingMessage) =>
        {
            Validators.EnshurePingModel(pingMessage.Body);
            var pingModel = new PingEventSource
            {
                //    ExecutionId = pingMessage.Header.ExecutionId,
                //    CorrelateId = pingMessage.Header.CorrelateId,
                //    Timestamp = pingMessage.Header.Timestamp,

                //    Sender = pingMessage.Footer.Sender,
                //    Route = pingMessage.Footer.Route,
                //    Environemnt = pingMessage.Footer.Environemnt,
                //    Assembly = pingMessage.Footer.Assembly,
                //    FingerPrint = pingMessage.Footer.FingerPrint,
                //    Hint = pingMessage.Footer.Hint,
                Data = JObject.FromObject(pingMessage.Body)
            };
            fillHeader(pingMessage.Header, pingModel);
            fillFooter(pingMessage.Footer, pingModel);
            return pingModel;
        };
    }
}
