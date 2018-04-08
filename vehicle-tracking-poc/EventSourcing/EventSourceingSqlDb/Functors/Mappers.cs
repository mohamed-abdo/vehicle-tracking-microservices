using BuildingAspects.Behaviors;
using DomainModels.Types.Messages;
using EventSourceingSqlDb.DbModels;
using Newtonsoft.Json;
using System;

namespace EventSourceingSqlDb.Functors
{

    public class Mappers<T> where T : new()
    {

        public static Func<Func<(MessageHeader header, T body, MessageFooter footer), bool>, Func<DbModel, bool>> PredicateMapper = (pingPredicate) =>
        {
            return (pingDbContext) =>
            {
                return pingPredicate(FromEnityToPingModel(pingDbContext));
            };
        };

        public static Func<(MessageHeader header, T body, MessageFooter footer), DbModel> FromPingModelToEnity = (pingMessage) =>
        {
            Validators<T>.EnshureModel(pingMessage.body);
            return DbModelFactory.Create(pingMessage.header, pingMessage.body, pingMessage.footer);
        };

        public static Func<DbModel, (MessageHeader Header, T Body, MessageFooter Footer)> FromEnityToPingModel = (pingEntity) =>
        {
            var header = new MessageHeader(executionId: pingEntity.ExecutionId, timestamp: pingEntity.Timestamp, isSucceed: true);
            var body = pingEntity.Data.ToObject<T>();
            var footer = new MessageFooter
            {
                Assembly = pingEntity.Assembly,
                Environment = pingEntity.Environment,
                FingerPrint = pingEntity.FingerPrint,
                Hint = Enum.GetName(typeof(ResponseHint), pingEntity.Hint),
                Route = JsonConvert.SerializeObject(pingEntity.Route, Defaults.JsonSerializerSettings),
                Sender = pingEntity.Sender
            };
            return (header, body, footer);
        };
    }
}
