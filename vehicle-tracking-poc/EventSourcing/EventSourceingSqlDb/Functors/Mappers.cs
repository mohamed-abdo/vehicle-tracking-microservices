using DomainModels.Types.Messages;
using EventSourceingSqlDb.DbModels;
using System;

namespace EventSourceingSqlDb.Functors
{
    public static class Mappers<T, R> where T : class where R : DbModel
    {

        public static Func<Func<(MessageHeader header, T body, MessageFooter footer), bool>, Func<R, bool>> PredicateMapper = (pingPredicate) =>
        {
            return (pingDbContext) =>
            {
                return pingPredicate(FromEnityToPingModel(pingDbContext));
            };
        };

        public static Func<(MessageHeader header, T body, MessageFooter footer), R> FromPingModelToEnity = (pingMessage) =>
        {
            Validators<T>.EnshureModel(pingMessage.body);

            return (R)DbModelFactory.Create(pingMessage.header, pingMessage.footer, pingMessage.body);
        };

        public static Func<R, (MessageHeader Header, T Body, MessageFooter Footer)> FromEnityToPingModel = (pingEntity) =>
        {
            var header = new MessageHeader(executionId: pingEntity.ExecutionId, timestamp: pingEntity.Timestamp, isSucceed: true);
            var body = pingEntity.Data.ToObject<T>();
            var footer = new MessageFooter
            {
                Assembly = pingEntity.Assembly,
                Environment = pingEntity.Environment,
                FingerPrint = pingEntity.FingerPrint,
                Hint = pingEntity.Hint,
                Route = pingEntity.Route,
                Sender = pingEntity.Sender
            };
            return (header, body, footer);
        };
    }
}
