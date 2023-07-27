using Assessment.Data;
using Assessment.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Business;

public class CloseDataRetrievalService : ICloseDataRetrievalService
{
    private readonly AssessmentDbContext context;

    public CloseDataRetrievalService(AssessmentDbContext context)
    {
        this.context = context;
    }

    public async Task<List<IngestedCloseResponse>> GetCloseApiResponses(int startPoint, int endPoint)
    {
        var fetchedCloseValueResponse = await context.CloseApiResponses
            .Include(x => x.ApiSource)
            .Where(x => x.StartPoint >= startPoint && x.StartPoint <= endPoint)
            .Select(x => new IngestedCloseResponse
            {
                PointInTime = x.StartPoint,
                Source = x.ApiSource.ApiName,
                Value = x.Close
            })
            .ToListAsync();

        return fetchedCloseValueResponse;
    }

    public async Task<List<AggregatedCloseResponse>> GetCloseAggregates(int startPoint, int endPoint)
    {
        var aggregatedCloseValueResponse = await context.CloseAggregates
            .Where(x => x.Id >= startPoint && x.Id <= endPoint)
            .Select(x => new AggregatedCloseResponse
            {
                PointInTime = x.Id,
                Method = x.Method,
                Value = x.AggregatedClose
            })
            .ToListAsync();

        return aggregatedCloseValueResponse;
    }
}