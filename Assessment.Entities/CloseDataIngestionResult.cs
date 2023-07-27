namespace Assessment.Entities
{
    public class CloseDataIngestionResult
    {
        public int ApiSourceId { get; set; }
        public double? Close { get; set; }
        public bool IsError { get; set; }
    }
}
