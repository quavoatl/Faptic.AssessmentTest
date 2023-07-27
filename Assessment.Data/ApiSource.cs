namespace Assessment.Data;

public class ApiSource
{
    public ApiSource()
    {
        CloseApiResponses = new HashSet<CloseApiResponse>();
        UrlConfigurations = new HashSet<UriConfiguration>();
    }

    public int Id { get; set; }
    public string? ApiName { get; set; }

    public ICollection<UriConfiguration>? UrlConfigurations { get; set; }
    public ICollection<CloseApiResponse>? CloseApiResponses { get; set; }
}