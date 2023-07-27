using Assessment.Entities;

namespace Assessment.Business.CloseDataIngestion
{
    public interface ICloseDataIngestionHandler
    {
        Task<CloseDataIngestionResult> GetCloseDataIngestionResult(int startPoint);
    }
}