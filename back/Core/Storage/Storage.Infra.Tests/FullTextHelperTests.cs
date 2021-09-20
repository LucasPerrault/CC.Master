using FluentAssertions;
using Storage.Infra.Querying;
using System.Linq;
using Xunit;

namespace Storage.Infra.Tests
{
    public class FullTextHelperTests
    {

        [Theory]
        [InlineData(new [] {"aperture", "figgo" }, "\"aperture*\" AND \"figgo*\"")]
        [InlineData(new [] {"@", "/" }, "\"@*\" AND \"/*\"")]
        [InlineData(new [] {"\"" }, "\"\"*\"")]
        public void ShouldProperlyConvertToFullTextContainsPredicate(string[] clues, string expected)
        {
            clues.ToHashSet().ToFullTextContainsPredicate().Should().Be(expected);
        }
    }
}
