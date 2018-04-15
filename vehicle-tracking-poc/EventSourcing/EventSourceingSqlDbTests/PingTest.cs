using BuildingAspects.Behaviors;
using DomainModels.Types.Messages;
using DomainModels.Business;
using EventSourceingSqlDb.Adapters;
using EventSourceingSqlDb.DbModels;
using EventSourceingSqlDb.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourceingSqlDbTests
{
    [TestFixture]
    public class PingTest
    {
        private readonly Mock<ILoggerFactory> _loggerMoq;
        private readonly VehicleDbContext _dbContext;
        private readonly ICommandEventSourcingLedger<PingModel> _eventSourcingLedger;
        private readonly IQueryEventSourcingLedger<PingModel> _eventSourcingLedgerQuery;
        private PingModel message;
        public PingTest()
        {
            _loggerMoq = new Mock<ILoggerFactory>(MockBehavior.Loose);

            _dbContext = new VehicleDbContext(new DbContextOptionsBuilder<VehicleDbContext>()
                .UseInMemoryDatabase(nameof(VehicleDbContext))
                .Options);
            _eventSourcingLedger = new PingEventSourcingLedgerAdapter(_loggerMoq.Object, _dbContext);
            _eventSourcingLedgerQuery = new PingEventSourcingLedgerAdapter(_loggerMoq.Object, _dbContext);
        }

        [SetUp]
        public void SetUp()
        {
            message = new PingModel
            {
                Header = new MessageHeader(executionId: Guid.NewGuid().ToString(), timestamp: new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()),
                Body = new Ping(),
                Footer = new MessageFooter()
            };
        }
        [Test]
        public async Task TestAddPingEventSourcing()
        {
            var result = await _eventSourcingLedger.Add(message);
            Assert.IsTrue(result > 0, "Add ping failed, result is negative number.");
        }

        [Test]
        public async Task TestAddPingSourcingVerifyMessageExecId()
        {
            var execId = message.Header.ExecutionId;
            var result = await _eventSourcingLedger.Add(message);
            var messageRec = await _dbContext.PingEventSource
                .FirstOrDefaultAsync(p => p.ExecutionId == execId);
            Assert.IsNotNull(messageRec, "Add ping failed, can't find message by execution id.");
        }

        [Test]
        public async Task TestAddPingEventSourcingVerifyMessageBody()
        {
            var execId = message.Header.ExecutionId;
            var messageBody = new Ping { ChassisNumber = "Xyz-Vehicle!" };
            message.Body = messageBody;
            var result = await _eventSourcingLedger.Add(message);
            var messageRec = await _dbContext.PingEventSource.FirstOrDefaultAsync(p => p.ExecutionId == execId);
            var originalObj = messageRec.Data.ToObject<PingModel>();
            Assert.IsTrue(originalObj.EqualsByValue(messageBody), "Add ping failed, can't get the original body from the message.");
        }
        Func<PingModel, (MessageHeader header, Ping body, MessageFooter footer)> Convert = (domainModel) =>
              (domainModel.Header, domainModel.Body, domainModel.Footer);
        [Test]
        public async Task TestQueryPingEventSourcing()
        {
            var execId = message.Header.ExecutionId;
            var messageBody = new Ping() { ChassisNumber = "Xyz-Vehicle!" };
            var pingMessage = new PingModel
            {
                Header = message.Header,
                Body = messageBody,
                Footer = message.Footer
            };

            var dbObjec = new PingEventSourcing(EventSourceingSqlDb.Functors.Mappers<Ping>.FromPingModelToEnity(Convert(pingMessage)));

            _dbContext.PingEventSource.Add(dbObjec);
            await _dbContext.SaveChangesAsync();
            var messageRec = _eventSourcingLedgerQuery.Query(p => p.Header.ExecutionId == execId).FirstOrDefault();

            Assert.IsTrue(pingMessage.Body.EqualsByValue(messageRec.Body), "query ping failed, message has been tempered.");
        }

        [Test]
        public async Task TestQueryPingEventSourcingFullMessage()
        {
            var execId = message.Header.ExecutionId;
            var messageBody = new Ping() { ChassisNumber = "Xyz-Vehicle!" };
            var pingMessage = new PingModel
            {
                Header = message.Header,
                Body = messageBody,
                Footer = message.Footer
            };

            byte[] binObjSource = Utilities.JsonBinarySerialize(pingMessage);

            var dbObjec = new PingEventSourcing(EventSourceingSqlDb.Functors.Mappers<Ping>.FromPingModelToEnity(Convert(pingMessage)));

            _dbContext.PingEventSource.Add(dbObjec);
            await _dbContext.SaveChangesAsync();
            var messageRec = _eventSourcingLedgerQuery.Query(p => p.Header.ExecutionId == execId).FirstOrDefault();

            byte[] binObj = Utilities.JsonBinarySerialize(messageRec);

            Assert.IsTrue(binObjSource.SequenceEqual(binObj), "query ping failed,full message has been tempered.");
        }
    }
}
