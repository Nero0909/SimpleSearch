using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NSubstitute;
using SimpleSearch.Indexer.Application.Queries;
using SimpleSearch.Indexer.Shared;
using SimpleSearch.Indexer.Shared.Entities;
using SimpleSearch.Tests.Shared;
using Xunit;

namespace SimpleSearch.Indexer.Tests.Application.Queries
{
    public class SearchDocumentsByTagQueryHandlerTests
    {
        [Theory]
        [InlineData("KeyWord", "keyword")]
        [InlineData("KEYWORD", "keyword")]
        [InlineData("keyWord", "keyword")]
        [InlineData("keyword", "keyword")]
        public async Task ShouldSearchNormalizedTag(string tag, string expected)
        {
            // Assert
            var fixture = new TestFixture<SearchDocumentsByTagQueryHandler>();
            var query = new SearchDocumentsByTagQuery(tag);

            fixture.Freeze<ITokensRepository>().FindByTagAsync(expected, CancellationToken.None)
                .Returns(fixture.Create<TokenEntity>());

            // Act
            var result = await fixture.Sut.Handle(query, CancellationToken.None);

            // Assert
            result.Documents.Should().NotBeEmpty();

        }
    }
}