namespace Assessment.Business.Aggregation
{
    public interface IAggregationStrategy
    {
        Aggregate ExecuteStrategy(List<double?> inputData);
    }
}
