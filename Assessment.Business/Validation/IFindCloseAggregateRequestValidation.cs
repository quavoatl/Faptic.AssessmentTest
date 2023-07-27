using Assessment.Entities;

namespace Assessment.Business.Validation
{
    public interface IFindCloseAggregateRequestValidation : IValidator<string, FindCloseAggregateRequest>
    {
    }

    public class FindCloseAggregateRequestValidation : IFindCloseAggregateRequestValidation
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
}
