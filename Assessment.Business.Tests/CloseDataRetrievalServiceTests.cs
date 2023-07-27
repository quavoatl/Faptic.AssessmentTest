using Assessment.Data;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using System.Linq;

namespace Assessment.Business.Tests
{
    [TestClass]
    public class CloseDataRetrievalServiceTests
    {
        private readonly Mock<AssessmentDbContext> contextMock;

        private readonly CloseDataRetrievalService closeDataRetrievalService;

        public CloseDataRetrievalServiceTests()
        {
            contextMock = new Mock<AssessmentDbContext>();
            closeDataRetrievalService = new CloseDataRetrievalService(contextMock.Object);
        }

        [TestMethod]
        public async Task GetCloseApiResponses_WithValuesInDatabase_ShouldReturnListOfTwoElements()
        {
            var closeApiResponseSeed = new List<CloseApiResponse>
            {
                new CloseApiResponse
                {
                    Close = 17000,
                    StartPoint = 1672531200,
                    ApiSource = new ApiSource
                    {
                        ApiName = "Bitstamp"
                    }
                },
                new CloseApiResponse
                {
                    Close = 18000,
                    StartPoint = 1672534800,
                    ApiSource = new ApiSource
                    {
                        ApiName = "Bitfinex"
                    }
                },
                new CloseApiResponse
                {
                    Close = 19000,
                    StartPoint = 1672538400,
                    ApiSource = new ApiSource
                    {
                        ApiName = "Coinbase"
                    }
                }
            };

            contextMock
                .Setup(x => x.CloseApiResponses)
                .ReturnsDbSet(closeApiResponseSeed);

            var ingestedCloseResponseList = await closeDataRetrievalService.GetCloseApiResponses(1672531200, 1672534800);

            ingestedCloseResponseList.Should().HaveCount(2);
            ingestedCloseResponseList.ElementAtOrDefault(0).Value.Should().Be(17000);
            ingestedCloseResponseList.ElementAtOrDefault(1).Value.Should().Be(18000);
        }

        [TestMethod]
        public async Task GetCloseAggregates_WithValuesInDatabase_ShouldReturnListOfTwoElements()
        {
            var closeAggregateSeed = new List<CloseAggregate>
            {
               new CloseAggregate
               {
                   AggregatedClose = 17000,
                   Id = 1672531200,
                   Method = "AVG"
               },
               new CloseAggregate
               {
                   AggregatedClose = 18000,
                   Id = 1672534800,
                   Method = "AVG"
               },
               new CloseAggregate
               {
                   AggregatedClose = 19000,
                   Id = 1672538400,
                   Method = "AVG"
               }
            };

            contextMock
                .Setup(x => x.CloseAggregates)
                .ReturnsDbSet(closeAggregateSeed);

            var ingestedCloseResponseList = await closeDataRetrievalService.GetCloseAggregates(1672531200, 1672534800);

            ingestedCloseResponseList.Should().HaveCount(2);
            ingestedCloseResponseList.ElementAtOrDefault(0).Value.Should().Be(17000);
            ingestedCloseResponseList.ElementAtOrDefault(1).Value.Should().Be(18000);
        }
    }
}
