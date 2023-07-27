using Assessment.Business.Aggregation;
using FluentAssertions;

namespace Assessment.Business.Tests
{
    [TestClass]
    public class MaxAggregationStrategyTests
    {
        private readonly MaxAggregationStrategy maxAggregationStrategy;

        public MaxAggregationStrategyTests()
        {
            maxAggregationStrategy = new MaxAggregationStrategy();
        }

        [TestMethod]
        public void ExecuteStrategy_ThreeDoubles_ReturnsCorrectMax()
        {
            var doublesList = new List<double?> { 17000, 18000, 19000 };

            var average = maxAggregationStrategy.ExecuteStrategy(doublesList);

            average.Result.Should().Be(19000);
            average.Method.Should().Be("MAX");
        }
    }
}