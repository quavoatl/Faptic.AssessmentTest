using Assessment.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Assessment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportingController : ControllerBase
    {
        private readonly ILogger<ReportingController> logger;
        private readonly IHttpClientFactory httpClientFactory;

        public ReportingController(ILogger<ReportingController> logger, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<WeatherForecast>> Get(int start)
        {
            //start = 1672531200
            var client = httpClientFactory.CreateClient();

            //RETRIEVE DATA IF START IS ALREADY SAVED IN DB
            //SHOULD HAVE 2 TABLES, ONE TO SAVE THE FETCHED DATA, ONE TO SAVE THE AGGREGATE DATA

            //FETCH DATA

            //BITSTAMP
            //seconds
            var x = await client.GetAsync($"https://www.bitstamp.net/api/v2/ohlc/btcusd/?step=3600&limit=1&start={start}");
            var y = await x.Content.ReadAsStringAsync();
            var z = JsonConvert.DeserializeObject<BitstampApiResponse>(y);


            //BITFINEX
            //1672531200000 is milliseconds
            var startAsMilliseconds = (long)1672531200 * 1000;
            var endAsMilliseconds = startAsMilliseconds + 36000;
            var xx = await client.GetAsync($"https://api-pub.bitfinex.com/v2/candles/trade:1h:tBTCUSD/hist?start={startAsMilliseconds}&end={endAsMilliseconds}&limit=1");
            var yy = await xx.Content.ReadAsStringAsync();
            var zz = JsonConvert.DeserializeObject<List<List<object>>>(yy);
            var bitfinexClose = zz.SingleOrDefault().ElementAt(2);


            //COINBASE
            //seconds
            var endInSeconds = start + 3600;
            client.DefaultRequestHeaders.Add("User-Agent", "Assessment App");
            var xxx = await client.GetAsync($"https://api.exchange.coinbase.com/products/BTC-USD/candles?granularity=60&start={start}&end={endInSeconds}");
            var yyy = await xxx.Content.ReadAsStringAsync();
            var zzz = JsonConvert.DeserializeObject<List<List<object>>>(yyy);
            var coinbaseClose = zzz.LastOrDefault();
            var xax = coinbaseClose.ElementAt(4);

            //AGGREGATE DATA
            


            //SAVE AGGREGATE DATA TO DB





            return new List<WeatherForecast>();
        }
    }
}