using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using SimpleSearch.Analyzer.Functions.Application.Commands;
using SimpleSearch.Analyzer.Functions.Application.Settings;
using SimpleSearch.Messages;
using SimpleSearch.Storage.Blobs;
using Xunit;

namespace SimpleSearch.Analyzer.Functions.Tests.Application.Commands
{
    public class FragmentTextFileCommandHandlerTests
    {
        [Fact]
        public async Task ShouldFragmentTextProperly()
        {
            // Arrange
            var fixture = new TestFixture();

            var input = "All work and no play makes nikita a dull boy";

            var command = new FragmentTextFileCommand("id", input.Length, FileExtension.Txt, "file");

            fixture.Freeze<IOptions<FragmentationSettings>>().Value
                .Returns(new FragmentationSettings {ChunkSizeInBytes = 10});

            var distribution = new[] {10, 20, 30, 40};

            foreach (var i in distribution)
            {
                var blob = fixture.Freeze<IBlobStorage>();
                blob.ReadRangeToStreamAsync(command.UploadId, Arg.Do<Stream>(s =>
                    {
                        using var writer = new StreamWriter(s, leaveOpen: true);
                        writer.Write(input[i..]);
                    }), i, Arg.Any<long>(), CancellationToken.None)
                    .Returns(true);
            }

            // Act
            var result = (await fixture.Sut.Handle(command, CancellationToken.None)).ToList();

            // Assert
            var ranges = result.Select(x => new {From = (int) x.Offset, To = (int) (x.Offset + x.Length)}).ToList();

            input[ranges[0].From..ranges[0].To].Should().Be("All work and");
            input[ranges[1].From..ranges[1].To].Should().Be("no play");
            input[ranges[2].From..ranges[2].To].Should().Be("makes nikita");
            input[ranges[3].From..ranges[3].To].Should().Be("a dull");
            input[ranges[4].From..ranges[4].To].Should().Be("boy");
        }

        private class TestFixture : Fixture
        {
            public TestFixture() => Customize(new AutoNSubstituteCustomization());

            public FragmentTextFileCommandHandler Sut => this.Create<FragmentTextFileCommandHandler>();
        }
    }
}