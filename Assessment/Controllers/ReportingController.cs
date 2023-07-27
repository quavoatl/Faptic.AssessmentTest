using Assessment.Business;
using Assessment.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class ReportingController : ControllerBase
    {
        private readonly ICloseDataIngestionService closeDataIngestionService;
        private readonly ICloseDataRetrievalService closeDataRetrievalService;

        public ReportingController(
            ICloseDataIngestionService closeDataIngestionService,
            ICloseDataRetrievalService closeDataRetrievalService)
        {
            this.closeDataIngestionService = closeDataIngestionService;
            this.closeDataRetrievalService = closeDataRetrievalService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FindCloseAggregateRequest request)
        {
            //request validator
           

            var result = await closeDataIngestionService.GetCloseDataAggregate(request);

            return Ok(result);
        }

        [HttpGet("[action]/{startPoint}/{endPoint}")]
        public async Task<List<IngestedCloseResponse>> GetIngestedClose(int startPoint, int endPoint)
        {
            var result = await closeDataRetrievalService.GetCloseApiResponses(startPoint, endPoint);

            return result;
        }

        [HttpGet("[action]/{startPoint}/{endPoint}")]
        public async Task<List<AggregatedCloseResponse>?> GetAggregatedClose(int startPoint, int endPoint)
        {
            var result = await closeDataRetrievalService.GetCloseAggregates(startPoint, endPoint);

            return result;
        }

    }
}