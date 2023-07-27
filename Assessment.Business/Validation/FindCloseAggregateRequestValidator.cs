using Assessment.Entities;

namespace Assessment.Business.Validation;

public class FindCloseAggregateRequestValidator : IFindCloseAggregateRequestValidator
{
    public string ValidateRequest(FindCloseAggregateRequest request)
    {
        var datetime = DateTime.UnixEpoch.AddSeconds(request.StartPoint);

        if (datetime.Minute != 0)
        {
            return "No start points with minutes allowed";
        }

        if (datetime.Second != 0)
        {
            return "No start points with seconds allowed";
        }

        return string.Empty;
    }
}