using Assessment.Business.CloseDataIngestion;
using Assessment.Data;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using FluentAssertions;
using Moq.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Assessment.Business.Tests
{
    [TestClass]
    public class CoinbaseCloseDataIngestionHandlerTests
    {

        private readonly Mock<AssessmentDbContext> contextMock;
        private readonly Mock<IHttpClientFactory> httpClientFactoryMock;
        private readonly Mock<ILogger<CoinbaseCloseDataIngestionHandler>> loggerMock;

        private readonly CoinbaseCloseDataIngestionHandler coinbaseCloseDataIngestionHandler;

        public CoinbaseCloseDataIngestionHandlerTests()
        {
            contextMock = new Mock<AssessmentDbContext>();
            loggerMock = new Mock<ILogger<CoinbaseCloseDataIngestionHandler>>();
            httpClientFactoryMock = new Mock<IHttpClientFactory>();

            var urlConfigurationSeed = new List<UriConfiguration>
            {
                new UriConfiguration(1,1,"Bitstamp1hCloseEndpoint","https://www.bitstamp.net/api/v2/ohlc/btcusd/?step=3600&limit=1&start=startPointPlaceholder"),
                new UriConfiguration(2,2,"Bitfinex1hCloseEndpoint","https://api-pub.bitfinex.com/v2/candles/trade:1h:tBTCUSD/hist?start=startPointPlaceholder&end=endPointPlaceholder&limit=1"),
                new UriConfiguration(3,3,"Coinbase1hCloseEndpoint","https://api.exchange.coinbase.com/products/BTC-USD/candles?granularity=60&start=startPointPlaceholder&end=endPointPlaceholder")
            };

            contextMock
                .Setup(x => x.UriConfigurations)
                .ReturnsDbSet(urlConfigurationSeed);

            coinbaseCloseDataIngestionHandler = new CoinbaseCloseDataIngestionHandler(
                httpClientFactoryMock.Object,
                loggerMock.Object,
                contextMock.Object);
        }

        [TestMethod]
        public async Task GetCloseDataIngestionResult_ValidStartPoint_ReturnsCorrectData()
        {
            var list = new List<object> { 1672531200, 16000, 17000, 18000, 17000 };
            var expectedApiResponse = JsonConvert.SerializeObject(new List<List<object>> { list });

            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedApiResponse)
                });

            var httpClient = new HttpClient(mockMessageHandler.Object);

            httpClientFactoryMock
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var result = await coinbaseCloseDataIngestionHandler.GetCloseDataIngestionResult(1672531200);

            result.Close.Should().NotBeNull();
            result.Close.Should().Be(17000);
            result.IsError.Should().Be(false);
        }

        [TestMethod]
        public async Task GetCloseDataIngestionResult_ValidStartPoint_ApiCallReturnsBadRequest()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                });

            var httpClient = new HttpClient(mockMessageHandler.Object);

            httpClientFactoryMock
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var result = await coinbaseCloseDataIngestionHandler.GetCloseDataIngestionResult(1672531200);

            result.Close.Should().BeNull();
            result.IsError.Should().Be(true);
        }

        [TestMethod]
        public async Task GetCloseDataIngestionResult_ValidStartPoint_ApiCallThrowsException()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Throw Exception")
                });

            var httpClient = new HttpClient(mockMessageHandler.Object);

            httpClientFactoryMock
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var result = await coinbaseCloseDataIngestionHandler.GetCloseDataIngestionResult(1672531200);

            result.Close.Should().BeNull();
            result.IsError.Should().Be(true);
            loggerMock.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((obj, type) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
