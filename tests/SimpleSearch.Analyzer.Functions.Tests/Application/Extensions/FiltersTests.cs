using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SimpleSearch.Analyzer.Functions.Application.Extensions;
using Xunit;

namespace SimpleSearch.Analyzer.Functions.Tests.Application.Extensions
{
    public class FiltersTests
    {
        [Fact]
        public void ShouldLowercaseStream()
        {
            var stream = "ThIsIsSomeStrinG";
            var expected = "thisissomestring";

            var result = new string(stream.ToCharArray().ToLowerCharacters().ToArray());

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(' ')]
        [InlineData('\n')]
        [InlineData('\t')]
        public void ShouldBeValidSeparator(char ch)
        {
            var isValid = ch.IsSeparator();

            isValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(@"\|!#$%&/()=?»«@£§€{}.-;'<>_,some text 42____", "some text 42")]
        public void ShouldFilterSpecialCharacters(string input, string expected)
        {
            var result = new string(input.ToCharArray().FilterSpecialCharacters().ToArray());

            result.Should().Be(expected);
        }

        [Fact]
        public void ShouldSplitWords()
        {
            var input = "  all work and no play makes nikita a dull boy  ";
            var expected = new[] {"all", "work", "and", "no", "play", "makes", "nikita", "a", "dull", "boy"};

            var result = input.ToCharArray().SplitWords().ToArray();

            result.Should().BeEquivalentTo(expected);
        }
    }
}