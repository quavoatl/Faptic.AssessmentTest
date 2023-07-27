using Assessment.Data;
using Assessment.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Assessment.Business.CloseDataIngestion
{
    public class CoinbaseCloseDataIngestionHandler : ICloseDataIngestionHandler
    {
        private readonly AssessmentDbContext context;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<CoinbaseCloseDataIngestionHandler> logger;

        public CoinbaseCloseDataIngestionHandler(
            IHttpClientFactory httpClientFactory,
            ILogger<CoinbaseCloseDataIngestionHandler> logger,
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
                ApiSourceId = 3
            };

            using var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Assessment App");

            var endPoint = startPoint + 3600;

            var uri = await context.UriConfigurations.FirstOrDefaultAsync(x => x.PropertyKey == "Coinbase1hCloseEndpoint");
            var requestUri = uri.PropertyValue
                .Replace("startPointPlaceholder", startPoint.ToString())
                .Replace("endPointPlaceholder", endPoint.ToString());

            var httpResponse = await client.GetAsync(requestUri);

            if (httpResponse.IsSuccessStatusCode == false)
            {
                closeDataIngestionResult.IsError = true;
                return closeDataIngestionResult;
            }

            var response = await httpResponse.Content.ReadAsStringAsync();

            try
            {
                var coinbaseApiResponse = JsonConvert.DeserializeObject<List<List<object>>>(response);

                var closeObject = coinbaseApiResponse.LastOrDefault().ElementAtOrDefault(4);
                closeDataIngestionResult.Close = Convert.ToDouble(closeObject);
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
