using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using NSubstitute;
using SimpleSearch.Analyzer.Functions.Application.Commands;
using SimpleSearch.Messages;
using SimpleSearch.Storage.Blobs;
using SimpleSearch.Tests.Shared;
using Xunit;

namespace SimpleSearch.Analyzer.Functions.Tests.Application.Commands
{
    public class TokenizeTextFileFragmentCommandHandlerTests
    {
        [Fact]
        public async Task ShouldTokenizeText()
        {
            // Arrange
            var fixture = new TestFixture<TokenizeTextFileFragmentCommandHandler>();

            var text = "112233445566778899 Saturn V rocket’s first stage carries 203,400 gallons " +
                       "(770,000 liters) " +
                       "of kerosene fuel and 318,000 gallons " +
                       "(1.2 million liters) of liquid oxygen needed for combustion. " +
                       "At liftoff, the stage’s five F-1 rocket engines ignite and " +
                       "produce 7.5 million pounds of thrust.";

            var tokens = new[]
            {
                "112233445566778899", "saturn", "v", "rockets", "first", "stage", "carries", "203400", "gallons",
                "770000", "liters",
                "of", "kerosene", "fuel", "and", "318000",
                "12", "million", "liquid", "oxygen", "needed", "for", "combustion",
                "at", "liftoff", "the", "stages", "five", "f1", "rocket", "engines", "ignite",
                "produce", "75", "pounds", "thrust"
            };

            var command = new TokenizeTextFileFragmentCommand("1", "file", FileExtension.Txt, 0, text.Length);

            fixture.Freeze<IBlobStorage>()
                .ReadRangeToStreamAsync(command.UploadId, Arg.Do<Stream>(s =>
                {
                    using var writer = new StreamWriter(s, leaveOpen: true);
                    writer.Write(text);
                }), Arg.Any<long>(), Arg.Any<long>(), CancellationToken.None)
                .Returns(true);

            // Act
            var result = await fixture.Sut.Handle(command, CancellationToken.None);

            // Assert
            result.Tokens.Select(x => x.Tag).Should().BeEquivalentTo(tokens);
        }
    }
}