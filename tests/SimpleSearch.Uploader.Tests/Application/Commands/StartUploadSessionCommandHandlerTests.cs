using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using SimpleSearch.Uploader.Application.Commands;
using SimpleSearch.Uploader.Application.Entities;
using SimpleSearch.Uploader.Application.Settings;
using SimpleSearch.Uploader.ClientResponses;
using Xunit;

namespace SimpleSearch.Uploader.Tests.Application.Commands
{
    public class StartUploadSessionCommandHandlerTests
    {
        [Fact]
        public async Task ShouldGenerateSessionWith()
        {
            // Arrange
            var fixture = new TestFixture();
            var command = new StartUploadSessionCommand("file", 10, "txt");

            fixture.Freeze<IOptions<UploadSettings>>().Value.Returns(new UploadSettings {ChunkSizeInBytes = 3});

            var expected = new StartUploadSessionResponse
            {
                Extension = "txt",
                FileName = "file",
                SizeInBytes = 10,
                Parts = new[]
                {
                    new UploadPart {Offset = 0, SizeInBytes = 3},
                    new UploadPart {Offset = 3, SizeInBytes = 3},
                    new UploadPart {Offset = 6, SizeInBytes = 3},
                    new UploadPart {Offset = 9, SizeInBytes = 1}
                }
            };

            // Act
            var result = await fixture.Sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expected,
                opt => opt.Excluding(x => x.SelectedMemberPath.EndsWith("Id")));
        }

        private class TestFixture : Fixture
        {
            public TestFixture() => Customize(new AutoNSubstituteCustomization());

            public StartUploadSessionCommandHandler Sut => this.Create<StartUploadSessionCommandHandler>();
        }
    }
}