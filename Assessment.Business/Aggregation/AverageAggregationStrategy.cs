namespace Assessment.Business.Aggregation;

public class AverageAggregationStrategy : IAggregationStrategy
{
    public Aggregate ExecuteStrategy(List<double?> inputData)
    {
        var filteredInput = inputData.Where(x => x.HasValue);

        var averageAggregateResult = filteredInput.Average();

        return new Aggregate
        {
            Result = averageAggregateResult,
            Method = "AVG"
        };
    }
}