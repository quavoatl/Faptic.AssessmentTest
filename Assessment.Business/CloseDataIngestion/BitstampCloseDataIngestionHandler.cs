using Assessment.Data;
using Assessment.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Assessment.Business.CloseDataIngestion
{
    public class BitstampCloseDataIngestionHandler : ICloseDataIngestionHandler
    {
        private readonly AssessmentDbContext context;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<BitstampCloseDataIngestionHandler> logger;

        public BitstampCloseDataIngestionHandler(
            IHttpClientFactory httpClientFactory,
            ILogger<BitstampCloseDataIngestionHandler> logger,
            AssessmentDbContext context)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
            this.context = context;
        }

        public async Task<CloseDataIngestionResult> GetCloseDataIngestionResult(int startPoint)
        {
            var closeDataIngestionResult = new CloseDataIngestionResult
            {
                ApiSourceId = 1
            };

            using var client = httpClientFactory.CreateClient();

            var uri = await context.UriConfigurations.FirstOrDefaultAsync(x => x.PropertyKey == "Bitstamp1hCloseEndpoint");
            var requestUri = uri.PropertyValue.Replace("startPointPlaceholder", startPoint.ToString());

            var httpResponse = await client.GetAsync(requestUri);

            if (httpResponse.IsSuccessStatusCode == false)
            {
                closeDataIngestionResult.IsError = true;
                return closeDataIngestionResult;
            }

            var response = await httpResponse.Content.ReadAsStringAsync();

            try
            {
                var bitstampApiResponse = JsonConvert.DeserializeObject<BitstampApiResponse>(response);

                closeDataIngestionResult.Close = bitstampApiResponse?.Data?.Ohlc?.FirstOrDefault()?.Close;
                return closeDataIngestionResult;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);

                closeDataIngestionResult.IsError = true;
                return closeDataIngestionResult;
            }
        }
    }
}
