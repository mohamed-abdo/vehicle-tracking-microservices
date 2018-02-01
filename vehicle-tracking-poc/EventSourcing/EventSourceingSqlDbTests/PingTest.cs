using BuildingAspects.Behaviors;
using DomainModels.Types.Messages;
using DomainModels.Vehicle;
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
        private readonly IEventSourcingLedger<(MessageHeader header, PingModel body, MessageFooter footer)> _eventSourcingLedger;

        private (MessageHeader header, PingModel body, MessageFooter footer) message;
        public PingTest()
        {
            _loggerMoq = new Mock<ILoggerFactory>(MockBehavior.Loose);

            _dbContext = new VehicleDbContext(new DbContextOptionsBuilder<VehicleDbContext>()
                .UseInMemoryDatabase(nameof(VehicleDbContext))
                .Options);
            _eventSourcingLedger = new PingEventSourcingLedgerAdapter(_loggerMoq.Object, _dbContext);
        }

        [SetUp]
        public void SetUp()
        {
            message = (
            header: new MessageHeader(executionId: Guid.NewGuid(), timestamp: new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()),
            body: new PingModel(),
            footer: new MessageFooter()
            );
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
            var execId = message.header.ExecutionId;
            var result = await _eventSourcingLedger.Add(message);
            var messageRec = await _dbContext.PingEventSource
                .FirstOrDefaultAsync(p => p.ExecutionId == execId);
            Assert.IsNotNull(messageRec, "Add ping failed, can't find message by execution id.");
        }

        [Test]
        public async Task TestAddPingEventSourcingVerifyMessageBody()
        {
            var execId = message.header.ExecutionId;
            var messageBody = new PingModel { ChassisNumber = "Xyz-Vehicle!" };
            message.body = messageBody;
            var result = await _eventSourcingLedger.Add(message);
            var messageRec = await _dbContext.PingEventSource.FirstOrDefaultAsync(p => p.ExecutionId == execId);
            var originalObj = messageRec.Data.ToObject<PingModel>();
            Assert.IsTrue(originalObj.EqualsByValue(messageBody), "Add ping failed, can't get the original body from the message.");
        }

        [Test]
        public async Task TestQueryPingEventSourcing()
        {
            var execId = message.header.ExecutionId;
            var messageBody = new PingModel() { ChassisNumber = "Xyz-Vehicle!" };
            var pingMessage = (
                            header: message.header,
                            body: messageBody,
                            footer: message.footer
                       );
            var dbObjec = new PingEventSourcing(EventSourceingSqlDb.Functors.Mappers<PingModel>.FromPingModelToEnity(pingMessage));

            _dbContext.PingEventSource.Add(dbObjec);
            await _dbContext.SaveChangesAsync();
            var messageRec = _eventSourcingLedger.Query(p => p.header.ExecutionId == execId).FirstOrDefault();

            Assert.IsTrue(pingMessage.body.EqualsByValue(messageRec.body), "query ping failed, message has been tempered.");
        }

        [Test]
        public async Task TestQueryPingEventSourcingFullMessage()
        {
            var execId = message.header.ExecutionId;
            var messageBody = new PingModel() { ChassisNumber = "Xyz-Vehicle!" };
            var pingMessage = (
                            header: message.header,
                            body: messageBody,
                            footer: message.footer
                       );

            byte[] binObjSource = Utilities.BinarySerialize(pingMessage);

            var dbObjec = new PingEventSourcing(EventSourceingSqlDb.Functors.Mappers<PingModel>.FromPingModelToEnity(pingMessage));

            _dbContext.PingEventSource.Add(dbObjec);
            await _dbContext.SaveChangesAsync();
            var messageRec = _eventSourcingLedger.Query(p => p.header.ExecutionId == execId).FirstOrDefault();

            byte[] binObjDestination = Utilities.BinarySerialize(messageRec);

            Assert.IsTrue(binObjSource.SequenceEqual(binObjDestination), "query ping failed,full message has been tempered.");
        }
    }
}
