using Instances.Domain.Instances;
using Xunit;

namespace Instances.Domain.Tests
{
    public class SubdomainExtensionsTests
    {
        [InlineData("aperture-science", "aperture-science")]
        [InlineData("Aperture Science", "aperture-science")]
        [InlineData("Àpêrture Scìënçe", "aperture-science")]
        [InlineData("Aperture   Science", "aperture-science")]
        [InlineData("Aperture/*-+/\\'{~:;,-Science", "aperture-science")]
        [InlineData("Aperture-Science/*-+/\\'{~:;,-", "aperture-science")]
        [InlineData("/*-+/\\'{~:;,-Aperture-Science", "aperture-science")]
        [InlineData("Aperture 🍌  Science", "aperture-science")]
        [InlineData("longer-than-63-aperture-science-aperture-science-aperture-science-aperture-science-aperture", "longer-than-63-aperture-science-aperture-science-aperture-scien")]
        [Theory]
        public void ShouldGiveValidSubdomain(string input, string expectedOutput)
        {
            Assert.Equal(expectedOutput, input.ToValidSubdomain());
        }
    }
}
