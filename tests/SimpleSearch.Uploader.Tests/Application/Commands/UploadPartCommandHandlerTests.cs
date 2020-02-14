using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using NSubstitute;
using SimpleSearch.Storage.Blobs;
using SimpleSearch.Tests.Shared;
using SimpleSearch.Uploader.Application.Commands;
using SimpleSearch.Uploader.Application.Entities;
using SimpleSearch.Uploader.Application.Repositories;
using Xunit;

namespace SimpleSearch.Uploader.Tests.Application.Commands
{
    public class UploadPartCommandHandlerTests
    {
        [Fact]
        public async Task ShouldReturnFalseIfSessionIsNotFound()
        {
            // Arrange
            var fixture = new TestFixture<UploadPartCommandHandler>();
            var command = new UploadPartCommand(new byte[0], "1", "1");

            // Act
            var result = await fixture.Sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ShouldReturnTrueIfSessionWasUploaded()
        {
            // Arrange
            var fixture = new TestFixture<UploadPartCommandHandler>();
            var data = new byte[0];
            var command = new UploadPartCommand(data, "1", "1");

            fixture.Freeze<ISessionsRepository>().FindNotCompletedSessionAsync(command.UploadId, CancellationToken.None)
                .Returns(fixture.Create<UploadSession>());

            fixture.Freeze<IBlobStorage>().UploadBlockBlob(data, "1", "1", CancellationToken.None).Returns(true);

            // Act
            var result = await fixture.Sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }
    }
}