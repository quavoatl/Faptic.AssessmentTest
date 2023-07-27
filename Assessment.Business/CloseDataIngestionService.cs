using Assessment.Business.Aggregation;
using Assessment.Business.CloseDataIngestion;
using Assessment.Data;
using Assessment.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Assessment.Business;

public class CloseDataIngestionService : ICloseDataIngestionService
{
    private readonly AssessmentDbContext context;
    private readonly IEnumerable<ICloseDataIngestionHandler> closeDataIngestionHandlers;
    private readonly ILogger<CloseDataIngestionService> logger;
    private readonly IAggregationStrategy aggregationStrategy;

    public CloseDataIngestionService(
        IEnumerable<ICloseDataIngestionHandler> closeDataIngestionHandlers,
        ILogger<CloseDataIngestionService> logger,
        AssessmentDbContext context,
        IAggregationStrategy aggregationStrategy)
    {
        this.closeDataIngestionHandlers = closeDataIngestionHandlers;
        this.logger = logger;
        this.context = context;
        this.aggregationStrategy = aggregationStrategy;
    }


    public async Task<CloseDataAggregateResponse> GetCloseDataAggregate(FindCloseAggregateRequest request)
    {
        var databaseCloseAggregate = await context.CloseAggregates
            .FirstOrDefaultAsync(x => x.Id == request.StartPoint);

        if (databaseCloseAggregate != null)
        {
            return new CloseDataAggregateResponse
            {
                Value = databaseCloseAggregate.AggregatedClose,
                Method = databaseCloseAggregate.Method
            };
        }

        var closeDataIngestionResultList = new List<CloseDataIngestionResult>();

        foreach (var handler in closeDataIngestionHandlers)
        {
            var handlerResult = await handler.GetCloseDataIngestionResult(request.StartPoint);
            closeDataIngestionResultList.Add(handlerResult);
        }

        HandleErrors(closeDataIngestionResultList);

        var aggregate = aggregationStrategy.ExecuteStrategy(closeDataIngestionResultList
            .Select(x => x.Close)
            .ToList());

        var closeApiResponsesToPersist = closeDataIngestionResultList
            .Select(x => new CloseApiResponse
            {
                ApiSourceId = x.ApiSourceId,
                Close = x.Close ?? null,
                StartPoint = request.StartPoint,
                IsError = x.IsError,
            });

        var closeAggregateToPersist = new CloseAggregate
        {
            Id = request.StartPoint,
            AggregatedClose = aggregate.Result.GetValueOrDefault(),
            Method = aggregate.Method
        };

        await context.CloseApiResponses.AddRangeAsync(closeApiResponsesToPersist);
        await context.CloseAggregates.AddAsync(closeAggregateToPersist);

        await context.SaveChangesAsync();

        return new CloseDataAggregateResponse
        {
            Value = aggregate.Result,
            Method = aggregate.Method
        };
    }

    private void HandleErrors(List<CloseDataIngestionResult> closeDataIngestionResultList)
    {
        if (closeDataIngestionResultList.Count(x => x.IsError) == 0)
        {
            return;
        }

        var errorList = closeDataIngestionResultList
            .Where(x => x.IsError)
            .Select(x => x.ApiSourceId);

        var ids = string.Join(",", errorList);
        logger.LogInformation($"Api Sources with id(s):{ids} failed to process the request");
    }
}