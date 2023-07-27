namespace Assessment.Business.Aggregation;

public class MaxAggregationStrategy : IAggregationStrategy
{
    public Aggregate ExecuteStrategy(List<double?> inputData)
    {
        var filteredInput = inputData.Where(x => x.HasValue);

        var maxAggregateResult = filteredInput.Max();

        return new Aggregate
        {
            Result = maxAggregateResult,
            Method = "MAX"
        };
    }
}