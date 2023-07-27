namespace Assessment.Entities;

public class IngestedCloseResponse
{
    public int PointInTime { get; set; }
    public string Source { get; set; }
    public double? Value { get; set; }
}