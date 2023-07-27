namespace Assessment.Data;

public class UriConfiguration
{
    public UriConfiguration(int id, int apiSourceId, string propertyKey, string propertyValue)
    {
        Id = id;
        ApiSourceId = apiSourceId;
        PropertyKey = propertyKey;
        PropertyValue = propertyValue;
    }

    public int Id { get; set; }
    public int ApiSourceId { get; set; }
    public string? PropertyKey { get; set; }
    public string? PropertyValue { get; set; }

    public ApiSource? ApiSource { get; set; }
}