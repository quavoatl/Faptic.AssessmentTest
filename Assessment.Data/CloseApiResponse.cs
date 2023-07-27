namespace Assessment.Data;

public class CloseApiResponse
{
    public int Id { get; set; }
    public int ApiSourceId { get; set; }
    public int StartPoint { get; set; }
    public double? Close { get; set; }
    public bool IsError { get; set; }

    public ApiSource? ApiSource { get; set; }
}