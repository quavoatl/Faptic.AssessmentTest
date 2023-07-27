using Assessment.Business.Validation;
using Assessment.Entities;
using FluentAssertions;

namespace Assessment.Business.Tests
{
    [TestClass]
    public class FindCloseAggregateRequestValidatorTests
    {
        private readonly FindCloseAggregateRequestValidator findCloseAggregateRequestValidator;

        public FindCloseAggregateRequestValidatorTests()
        {
            findCloseAggregateRequestValidator = new FindCloseAggregateRequestValidator();
        }

        [TestMethod]
        public void ValidateRequest_WithValidInput_ReturnsEmptyString()
        {
            var validationResult = findCloseAggregateRequestValidator.ValidateRequest(
                new FindCloseAggregateRequest
                {
                    StartPoint = 1672531200
                });

            validationResult.Should().Be(string.Empty);
        }

        [TestMethod]
        public void ValidateRequest_WithMinutes_ReturnsError()
        {
            var validationResult = findCloseAggregateRequestValidator.ValidateRequest(
                new FindCloseAggregateRequest
                {
                    StartPoint = 1672531280
                });

            validationResult.Should().Be("No start points with minutes allowed");
        }

        [TestMethod]
        public void ValidateRequest_WithSeconds_ReturnsError()
        {
            var validationResult = findCloseAggregateRequestValidator.ValidateRequest(
                new FindCloseAggregateRequest
                {
                    StartPoint = 1672531205
                });

            validationResult.Should().Be("No start points with seconds allowed");
        }
    }
}
