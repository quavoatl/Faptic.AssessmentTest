using Assessment.Entities;

namespace Assessment.Business
{
    public interface ICloseDataIngestionService
    {
        Task<CloseDataAggregateResponse> GetCloseDataAggregate(FindCloseAggregateRequest request);
    }
}
