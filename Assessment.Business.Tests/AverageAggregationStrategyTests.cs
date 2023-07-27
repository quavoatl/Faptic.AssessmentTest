using Assessment.Business.Aggregation;
using FluentAssertions;

namespace Assessment.Business.Tests
{
    [TestClass]
    public class AverageAggregationStrategyTests
    {
        private readonly AverageAggregationStrategy averageAggregationStrategy;

        public AverageAggregationStrategyTests()
        {
            averageAggregationStrategy = new AverageAggregationStrategy();
        }

        [TestMethod]
        public void ExecuteStrategy_ThreeDoubles_ReturnsCorrectAverage()
        {
            var doublesList = new List<double?> { 17000, 18000, 19000 };

            var average = averageAggregationStrategy.ExecuteStrategy(doublesList);

            average.Result.Should().Be(18000);
            average.Method.Should().Be("AVG");
        }
    }
}