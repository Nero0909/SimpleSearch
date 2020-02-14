using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SimpleSearch.Indexer.Functions.Application.Commands;
using SimpleSearch.Indexer.Shared;
using SimpleSearch.Indexer.Shared.Entities;
using SimpleSearch.Messages;
using SimpleSearch.Tests.Shared;
using Xunit;

namespace SimpleSearch.Indexer.Functions.Tests.Application.Commands
{
    public class IndexFragmentCommandHandlerTests
    {
        [Fact]
        public async Task ShouldIndexDocuments()
        {
            // Arrange
            var fixture = new TestFixture<IndexFragmentCommandHandler>();
            var repo = fixture.Freeze<ITokensRepository>();

            var tokens = new[]
            {
                new Token {Tag = "first"},
                new Token {Tag = "second"}
            };
            var command = new IndexFragmentCommand("doc", "file", FileExtension.Txt, 0, 10, tokens);

            // Act
            await fixture.Sut.Handle(command, CancellationToken.None);

            // Assert
            await repo.Received()
                .AddDocumentToTokenAsync(
                    Arg.Is<DocumentEntity>(d => d.Id == "doc" && d.Name == "file" && d.Extension == "Txt"), "first",
                    CancellationToken.None);
            await repo.Received()
                .AddDocumentToTokenAsync(
                    Arg.Is<DocumentEntity>(d => d.Id == "doc" && d.Name == "file" && d.Extension == "Txt"), "second",
                    CancellationToken.None);
        }
    }
}