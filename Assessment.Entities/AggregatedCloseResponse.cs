namespace Assessment.Entities;

public class AggregatedCloseResponse
{
    public int PointInTime { get; set; }
    public string Method { get; set; }
    public double Value { get; set; }
}