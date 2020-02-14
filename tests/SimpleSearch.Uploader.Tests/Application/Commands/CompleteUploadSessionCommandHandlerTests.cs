using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using NSubstitute;
using SimpleSearch.Storage.Blobs;
using SimpleSearch.Uploader.Application.Commands;
using SimpleSearch.Uploader.Application.Entities;
using SimpleSearch.Uploader.Application.Repositories;
using SimpleSearch.Uploader.ClientRequests;
using SimpleSearch.Uploader.ClientResponses;
using Xunit;

namespace SimpleSearch.Uploader.Tests.Application.Commands
{
    public class CompleteUploadSessionCommandHandlerTests
    {
        [Fact]
        public async Task ShouldReturnNullIfSessionIsNotFound()
        {
            // Arrange
            var fixture = new TestFixture();
            var command = new CompleteUploadSessionCommand("123");

            // Act
            var result = await fixture.Sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ShouldCompleteSessionIfAllBlocksAreUploaded()
        {
            // Arrange
            var fixture = new TestFixture();
            var command = new CompleteUploadSessionCommand("123");

            var session = fixture.Build<UploadSession>()
                .With(x => x.Parts, new[]
                {
                    fixture.Build<UploadPart>().With(x => x.Id, "1").With(x => x.SizeInBytes, 10).Create(),
                    fixture.Build<UploadPart>().With(x => x.Id, "2").With(x => x.SizeInBytes, 20).Create()
                })
                .With(x => x.Id, "123")
                .With(x => x.Extension, FileExtension.Txt.ToString())
                .Create();

            fixture.Freeze<ISessionsRepository>().FindNotCompletedSessionAsync(command.UploadId, CancellationToken.None)
                .Returns(session);

            var blockList = new[]
            {
                new BlockInfo("1", 10), 
                new BlockInfo("2", 20)
            };

            fixture.Freeze<IBlobStorage>().GetBlockListAsync("123", CancellationToken.None)
                .Returns(blockList);

            // Act
            var result = await fixture.Sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CorruptedParts.Should().BeEmpty();
        }

        [Fact]
        public async Task ShouldGetCorruptedBlocksIfNoneOfThemWereNotUploaded()
        {
            // Arrange
            var fixture = new TestFixture();
            var command = new CompleteUploadSessionCommand("123");

            var session = fixture.Build<UploadSession>()
                .With(x => x.Parts, new[]
                {
                    fixture.Build<UploadPart>().With(x => x.Id, "1").With(x => x.SizeInBytes, 10).With(x => x.Offset, 0).Create()
                })
                .With(x => x.Id, "123")
                .With(x => x.Extension, FileExtension.Txt.ToString())
                .Create();

            var expectedPart = new CorruptedPart
            {
                ActualSizeInBytes = 0, ExpectedSizeInBytes = 10, Offset = 0, Id = "1"
            };

            fixture.Freeze<ISessionsRepository>().FindNotCompletedSessionAsync(command.UploadId, CancellationToken.None)
                .Returns(session);

            // Act
            var result = await fixture.Sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CorruptedParts.Should().BeEquivalentTo(expectedPart);
        }

        [Fact]
        public async Task ShouldGetCorruptedBlocksIfSizeIsDifferent()
        {
            // Arrange
            var fixture = new TestFixture();
            var command = new CompleteUploadSessionCommand("123");

            var session = fixture.Build<UploadSession>()
                .With(x => x.Parts, new[]
                {
                    fixture.Build<UploadPart>().With(x => x.Id, "1").With(x => x.SizeInBytes, 10).With(x => x.Offset, 0).Create()
                })
                .With(x => x.Id, "123")
                .With(x => x.Extension, FileExtension.Txt.ToString())
                .Create();

            var blockList = new[]
            {
                new BlockInfo("1", 5)
            };

            fixture.Freeze<IBlobStorage>().GetBlockListAsync("123", CancellationToken.None)
                .Returns(blockList);

            var expectedPart = new CorruptedPart
            {
                ActualSizeInBytes = 5,
                ExpectedSizeInBytes = 10,
                Offset = 0,
                Id = "1"
            };

            fixture.Freeze<ISessionsRepository>().FindNotCompletedSessionAsync(command.UploadId, CancellationToken.None)
                .Returns(session);

            // Act
            var result = await fixture.Sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CorruptedParts.Should().BeEquivalentTo(expectedPart);
        }

        private class TestFixture : Fixture
        {
            public TestFixture() => Customize(new AutoNSubstituteCustomization());

            public CompleteUploadSessionCommandHandler Sut => this.Create<CompleteUploadSessionCommandHandler>();
        }
    }
}