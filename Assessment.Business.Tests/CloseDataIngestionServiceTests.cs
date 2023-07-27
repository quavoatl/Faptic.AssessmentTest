using System.Collections;
using Assessment.Business.Aggregation;
using Assessment.Business.CloseDataIngestion;
using Assessment.Data;
using Assessment.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;

namespace Assessment.Business.Tests
{
    [TestClass]
    public class CloseDataIngestionServiceTests
    {
        private readonly Mock<AssessmentDbContext> contextMock;
        private readonly Mock<ILogger<CloseDataIngestionService>> loggerMock;
        private readonly Mock<IAggregationStrategy> aggregationStrategyMock;

        private IEnumerable<ICloseDataIngestionHandler> closeDataIngestionHandlers;
        private CloseDataIngestionService closeDataIngestionService;

        public CloseDataIngestionServiceTests()
        {
            contextMock = new Mock<AssessmentDbContext>();
            loggerMock = new Mock<ILogger<CloseDataIngestionService>>();
            aggregationStrategyMock = new Mock<IAggregationStrategy>();
        }

        [TestMethod]
        public async Task GetCloseDataAggregate_WithValidStartPoint_ShouldReturnAggregateAndPersistEntities()
        {
            var insertedCloseAggregates = new List<CloseAggregate>();
            var insertedCloseApiResponses = new List<CloseApiResponse>();

            var bitstampMock = new Mock<ICloseDataIngestionHandler>();
            bitstampMock
                .Setup(x => x.GetCloseDataIngestionResult(It.IsAny<int>()))
                .ReturnsAsync(new CloseDataIngestionResult
                {
                    IsError = false,
                    Close = 17000,
                    ApiSourceId = 1
                });

            var bitfinexMock = new Mock<ICloseDataIngestionHandler>();
            bitfinexMock
                .Setup(x => x.GetCloseDataIngestionResult(It.IsAny<int>()))
                .ReturnsAsync(new CloseDataIngestionResult
                {
                    IsError = false,
                    Close = 18000,
                    ApiSourceId = 2
                });

            var coinbaseMock = new Mock<ICloseDataIngestionHandler>();
            coinbaseMock
                .Setup(x => x.GetCloseDataIngestionResult(It.IsAny<int>()))
                .ReturnsAsync(new CloseDataIngestionResult
                {
                    IsError = false,
                    Close = 19000,
                    ApiSourceId = 3
                });

            closeDataIngestionHandlers = new List<ICloseDataIngestionHandler> { bitstampMock.Object, bitfinexMock.Object, coinbaseMock.Object };

            closeDataIngestionService = new CloseDataIngestionService(
                closeDataIngestionHandlers,
                loggerMock.Object,
                contextMock.Object,
                aggregationStrategyMock.Object);


            var closeAggregateSeed = new List<CloseAggregate>();

            contextMock
                .Setup(x => x.CloseAggregates)
                .ReturnsDbSet(closeAggregateSeed);

            var closeApiResponsesSeed = new List<CloseApiResponse>();

            contextMock
                .Setup(x => x.CloseApiResponses)
                .ReturnsDbSet(closeApiResponsesSeed);

            aggregationStrategyMock
                .Setup(x => x.ExecuteStrategy(It.IsAny<List<double?>>()))
                .Returns(new Aggregate
                {
                    Method = "AVG",
                    Result = 18000
                });

            contextMock.Setup(m => m.CloseAggregates.AddAsync(It.IsAny<CloseAggregate>(), default))
                .Callback<CloseAggregate, CancellationToken>((s, token) => { insertedCloseAggregates.Add(s); });

            contextMock.Setup(m => m.CloseApiResponses.AddRangeAsync(It.IsAny<IEnumerable<CloseApiResponse>>(), default))
                .Callback<IEnumerable<CloseApiResponse>, CancellationToken>((s, token) => { insertedCloseApiResponses.AddRange(s); });

            contextMock.Setup(c => c.SaveChangesAsync(default))
                .Returns(Task.FromResult(1))
                .Verifiable();

            var closeDataAggregateResult = await closeDataIngestionService.GetCloseDataAggregate(new FindCloseAggregateRequest
            {
                StartPoint = 1672531200
            });

            closeDataAggregateResult.Should().NotBeNull();
            closeDataAggregateResult.Method.Should().Be("AVG");
            closeDataAggregateResult.Value.Should().Be(18000);
            insertedCloseAggregates.Should().HaveCount(1);
            insertedCloseApiResponses.Should().HaveCount(3);
        }

        [TestMethod]
        public async Task GetCloseDataAggregate_WithValidStartPoint_ShouldLogErrorsWhenOneApiCallFails()
        {
            var bitstampMock = new Mock<ICloseDataIngestionHandler>();
            bitstampMock
                .Setup(x => x.GetCloseDataIngestionResult(It.IsAny<int>()))
                .ReturnsAsync(new CloseDataIngestionResult
                {
                    IsError = true,
                    ApiSourceId = 1
                });

            var bitfinexMock = new Mock<ICloseDataIngestionHandler>();
            bitfinexMock
                .Setup(x => x.GetCloseDataIngestionResult(It.IsAny<int>()))
                .ReturnsAsync(new CloseDataIngestionResult
                {
                    IsError = false,
                    Close = 18000,
                    ApiSourceId = 2
                });

            var coinbaseMock = new Mock<ICloseDataIngestionHandler>();
            coinbaseMock
                .Setup(x => x.GetCloseDataIngestionResult(It.IsAny<int>()))
                .ReturnsAsync(new CloseDataIngestionResult
                {
                    IsError = false,
                    Close = 19000,
                    ApiSourceId = 3
                });

            closeDataIngestionHandlers = new List<ICloseDataIngestionHandler> { bitstampMock.Object, bitfinexMock.Object, coinbaseMock.Object };

            closeDataIngestionService = new CloseDataIngestionService(
                closeDataIngestionHandlers,
                loggerMock.Object,
                contextMock.Object,
                aggregationStrategyMock.Object);


            var closeAggregateSeed = new List<CloseAggregate>();

            contextMock
                .Setup(x => x.CloseAggregates)
                .ReturnsDbSet(closeAggregateSeed);

            var closeApiResponsesSeed = new List<CloseApiResponse>();

            contextMock
                .Setup(x => x.CloseApiResponses)
                .ReturnsDbSet(closeApiResponsesSeed);

            aggregationStrategyMock
                .Setup(x => x.ExecuteStrategy(It.IsAny<List<double?>>()))
                .Returns(new Aggregate
                {
                    Method = "AVG",
                    Result = 18000
                });

            contextMock.Setup(c => c.SaveChangesAsync(default))
                .Returns(Task.FromResult(1))
                .Verifiable();

            await closeDataIngestionService.GetCloseDataAggregate(new FindCloseAggregateRequest
            {
                StartPoint = 1672531200
            });

            loggerMock.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((obj, type) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task GetCloseDataAggregate_WithValidStartPoint_ShouldReturnPersistedValue()
        {
            closeDataIngestionHandlers = new List<ICloseDataIngestionHandler> { };

            closeDataIngestionService = new CloseDataIngestionService(
                closeDataIngestionHandlers,
                loggerMock.Object,
                contextMock.Object,
                aggregationStrategyMock.Object);

            var closeAggregateSeed = new List<CloseAggregate>
            {
                new CloseAggregate
                {
                    Id = 1672531200,
                    AggregatedClose = 17000,
                    Method = "AVG"
                }
            };

            contextMock
                .Setup(x => x.CloseAggregates)
                .ReturnsDbSet(closeAggregateSeed);
            
            var closeDataAggregateResult = await closeDataIngestionService.GetCloseDataAggregate(new FindCloseAggregateRequest
            {
                StartPoint = 1672531200
            });

            closeDataAggregateResult.Value.Should().Be(17000);
            closeDataAggregateResult.Method.Should().Be("AVG");

        }
    }
}
