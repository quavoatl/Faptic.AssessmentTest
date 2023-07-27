using Assessment.Entities;

namespace Assessment.Business
{
    public interface ICloseDataRetrievalService
    {
        Task<List<IngestedCloseResponse>> GetCloseApiResponses(int startPoint, int endPoint);
        Task<List<AggregatedCloseResponse>> GetCloseAggregates(int startPoint, int endPoint);
    }
}
