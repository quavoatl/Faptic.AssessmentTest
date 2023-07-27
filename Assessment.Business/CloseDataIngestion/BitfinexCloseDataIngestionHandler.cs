using Assessment.Data;
using Assessment.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Assessment.Business.CloseDataIngestion
{
    public class BitfinexCloseDataIngestionHandler : ICloseDataIngestionHandler
    {
        private readonly AssessmentDbContext context;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<BitfinexCloseDataIngestionHandler> logger;

        public BitfinexCloseDataIngestionHandler(
            IHttpClientFactory httpClientFactory,
            ILogger<BitfinexCloseDataIngestionHandler> logger,
            AssessmentDbContext context)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
            this.context = context;
        }

        /// <summary>
        /// Bitfinex start/end point are represented in milliseconds
        /// </summary>
        /// <param name="startPoint"></param>
        /// <returns></returns>
        public async Task<CloseDataIngestionResult> GetCloseDataIngestionResult(int startPoint)
        {
            var closeDataIngestionResult = new CloseDataIngestionResult
            {
                ApiSourceId = 2
            };

            using var client = httpClientFactory.CreateClient();

            var startPointInMilliseconds = (long)startPoint * 1000;
            var endPointInMilliseconds = startPointInMilliseconds + 3600 * 1000;

            var uri = await context.UriConfigurations.FirstOrDefaultAsync(x => x.PropertyKey == "Bitfinex1hCloseEndpoint");
            var requestUri = uri.PropertyValue
                .Replace("startPointPlaceholder", startPointInMilliseconds.ToString())
                .Replace("endPointPlaceholder", endPointInMilliseconds.ToString());

            var httpResponse = await client.GetAsync(requestUri);

            if (httpResponse.IsSuccessStatusCode == false)
            {
                closeDataIngestionResult.IsError = true;
                return closeDataIngestionResult;
            }

            var response = await httpResponse.Content.ReadAsStringAsync();

            try
            {
                var bitfinexApiResponse = JsonConvert.DeserializeObject<List<List<object>>>(response);

                var closeObject = bitfinexApiResponse.SingleOrDefault().ElementAtOrDefault(2);
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
