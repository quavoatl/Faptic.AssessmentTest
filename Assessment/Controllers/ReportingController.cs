using Assessment.Business;
using Assessment.Business.Validation;
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
        private readonly IFindCloseAggregateRequestValidator findCloseAggregateRequestValidator;

        public ReportingController(
            ICloseDataIngestionService closeDataIngestionService,
            ICloseDataRetrievalService closeDataRetrievalService,
            IFindCloseAggregateRequestValidator findCloseAggregateRequestValidator)
        {
            this.closeDataIngestionService = closeDataIngestionService;
            this.closeDataRetrievalService = closeDataRetrievalService;
            this.findCloseAggregateRequestValidator = findCloseAggregateRequestValidator;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FindCloseAggregateRequest request)
        {
            var validationResult = findCloseAggregateRequestValidator.ValidateRequest(request);

            if (string.IsNullOrEmpty(validationResult) == false)
            {
                return BadRequest(validationResult);
            }

            var aggregateResult = await closeDataIngestionService.GetCloseDataAggregate(request);

            return Ok(aggregateResult);
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