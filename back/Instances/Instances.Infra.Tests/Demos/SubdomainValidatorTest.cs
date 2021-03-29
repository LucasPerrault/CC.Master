using Environments.Domain;
using Environments.Domain.Storage;
using Instances.Domain.Demos;
using Instances.Infra.Demos;
using Lucca.Core.Shared.Domain.Exceptions;
using MockQueryable.Moq;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Infra.Tests.Demos
{
    public class SubdomainValidatorTest
    {
        private readonly Mock<IDemosStore> _demosStoreMock;
        private readonly Mock<IEnvironmentsStore> _envStoreMock;

        public SubdomainValidatorTest()
        {
            _demosStoreMock = new Mock<IDemosStore>();
            _envStoreMock = new Mock<IEnvironmentsStore>();
        }


        [Theory]
        [InlineData("aperture-science")]
        [InlineData("Aperture-science")]
        [InlineData("APERTURE-SCIENCE")]
        public void IsAvailableAsync_ShouldReturnTrue_WhenSubdomainIsNotTaken(string takenSubdomain)
        {

            var demos = new List<Demo>
            {
                new Demo { Subdomain = takenSubdomain, IsActive = false}
            };
            var envs = new List<Environment>
            {
                new Environment { Subdomain = takenSubdomain, IsActive = false}
            };

            _demosStoreMock.Setup(s => s.GetAll()).Returns(demos.AsQueryable().BuildMock().Object);
            _envStoreMock.Setup(s => s.GetAll()).Returns(envs.AsQueryable().BuildMock().Object);
            var subdomainValidator = new SubdomainValidator(_demosStoreMock.Object, _envStoreMock.Object);

            Assert.True(subdomainValidator.IsAvailable("aperture-science"));
        }

        [Theory]
        [InlineData("aperture-science")]
        [InlineData("Aperture-science")]
        [InlineData("APERTURE-SCIENCE")]
        public void  IsAvailableAsync_ShouldReturnFalse_WhenSubdomainIsTakenByActiveDemo(string takenSubdomain)
        {
            var demos = new List<Demo>
            {
                new Demo { Subdomain = takenSubdomain, IsActive = true}
            };
            var envs = new List<Environment>
            {
                new Environment { Subdomain = takenSubdomain, IsActive = false}
            };

            _demosStoreMock.Setup(s => s.GetAll()).Returns(demos.AsQueryable().BuildMock().Object);
            _envStoreMock.Setup(s => s.GetAll()).Returns(envs.AsQueryable().BuildMock().Object);
            var subdomainValidator = new SubdomainValidator(_demosStoreMock.Object, _envStoreMock.Object);

            Assert.False(subdomainValidator.IsAvailable("aperture-science"));
        }

        [Theory]
        [InlineData("aperture-science")]
        [InlineData("this-was-a-triumph-im-making-a-note-here-huge-success-its-hard-to-overstate-my-satisfaction-aperture-science-we-do-what-we-must-because-we-can-for-the-good-of-all-of-us-except-the-ones-who-are-dead-bu")]
        public async Task IsAvailableAsync_ShouldNotThrow_WhenSubdomainIsNotValid(string validSubdomain)
        {
            var subdomainValidator = new SubdomainValidator(_demosStoreMock.Object, _envStoreMock.Object);

            await subdomainValidator.ThrowIfInvalidAsync(validSubdomain);
        }

        [Theory]
        [InlineData("-aperture-science")]
        [InlineData("aperture#science")]
        [InlineData("aperture-scìence")]
        [InlineData("aperture\u1F91science")]
        [InlineData("this-was-a-triumph-im-making-a-note-here-huge-success-its-hard-to-overstate-my-satisfaction-aperture-science-we-do-what-we-must-because-we-can-for-the-good-of-all-of-us-except-the-ones-who-are-dead-but")]
        [InlineData("Aperture-science")]
        [InlineData("APERTURE-SCIENCE")]
        [InlineData("a")]
        [InlineData("cc")]
        [InlineData("iis-aperture")]
        public async Task IsAvailableAsync_ShouldThrow_WhenSubdomainIsNotValid(string invalidSubdomain)
        {
            var subdomainValidator = new SubdomainValidator(_demosStoreMock.Object, _envStoreMock.Object);

            await Assert.ThrowsAsync<BadRequestException>(async () => await subdomainValidator.ThrowIfInvalidAsync(invalidSubdomain));
        }
    }
}
