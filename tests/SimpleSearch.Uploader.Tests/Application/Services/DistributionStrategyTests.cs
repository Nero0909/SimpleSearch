using FluentAssertions;
using SimpleSearch.Uploader.Application.Services;
using Xunit;

namespace SimpleSearch.Uploader.Tests.Application.Services
{
    public class DistributionStrategyTests
    {
        [Theory]
        [InlineData(10, 3, new[] {3, 3, 3, 1})]
        [InlineData(10, 5, new[] {5, 5})]
        [InlineData(10, 7, new[] {7, 3})]
        [InlineData(10, 20, new[] {10})]
        public void ShouldDistributeBySize(int size, int chunkSize, int[] expected)
        {
            // Act
            var distribution = DistributionStrategy.BySize(size, chunkSize);

            // Assert
            distribution.Should().BeEquivalentTo(expected);
        }
    }
}