using Environments.Domain.Storage;
using FluentAssertions;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Filtering;
using Instances.Infra.Demos;
using Lucca.Core.Shared.Domain.Exceptions;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;
using Xunit;
using Environment = Environments.Domain.Environment;

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
        public async Task IsAvailableAsync_ShouldReturnTrue_WhenSubdomainIsNotTaken(string takenSubdomain)
        {

            var envs = new List<Environment>
            {
                new Environment { Subdomain = takenSubdomain, IsActive = false}
            };

            _demosStoreMock
                .Setup(s => s.GetAsync(It.Is<DemoFilter>(d => d.IsActive == BoolCombination.TrueOnly), It.IsAny<DemoAccess>()))
                .ReturnsAsync(new List<Demo>());
            _envStoreMock.Setup(s => s.GetAll()).Returns(envs.AsQueryable().BuildMock().Object);
            var subdomainValidator = new SubdomainValidator(_demosStoreMock.Object, _envStoreMock.Object);

            Assert.True(await subdomainValidator.IsAvailableAsync("aperture-science"));
        }

        [Theory]
        [InlineData("aperture-science")]
        [InlineData("Aperture-science")]
        [InlineData("APERTURE-SCIENCE")]
        public async Task IsAvailableAsync_ShouldReturnFalse_WhenSubdomainIsTakenByActiveDemo(string takenSubdomain)
        {
            var demos = new List<Demo> { new Demo { Subdomain = takenSubdomain } };

            _demosStoreMock
                .Setup(s => s.GetAsync(It.Is<DemoFilter>(d => d.IsActive == BoolCombination.TrueOnly), It.IsAny<DemoAccess>()))
                .ReturnsAsync(demos);
            _envStoreMock.Setup(s => s.GetAll()).Returns(new List<Environment>().AsQueryable().BuildMock().Object);
            var subdomainValidator = new SubdomainValidator(_demosStoreMock.Object, _envStoreMock.Object);

            Assert.False(await subdomainValidator.IsAvailableAsync("aperture-science"));
        }

        [Theory]
        [InlineData("aperture-science")]
        [InlineData("Aperture-science")]
        [InlineData("APERTURE-SCIENCE")]
        public async Task IsAvailableAsync_ShouldReturnFalse_WhenSubdomainIsTakenByActiveEnv(string takenSubdomain)
        {
            var envs = new List<Environment>
            {
                new Environment { Subdomain = takenSubdomain, IsActive = true}
            };

            _demosStoreMock
                .Setup(s => s.GetAsync(It.Is<DemoFilter>(d => d.IsActive == BoolCombination.TrueOnly), It.IsAny<DemoAccess>()))
                .ReturnsAsync(new List<Demo>());

            _envStoreMock.Setup(s => s.GetAll()).Returns(envs.AsQueryable().BuildMock().Object);
            var subdomainValidator = new SubdomainValidator(_demosStoreMock.Object, _envStoreMock.Object);

            Assert.False(await subdomainValidator.IsAvailableAsync("aperture-science"));
        }

        [Theory]
        [InlineData("aperture-science")]
        [InlineData("not-longer-than-63-this-was-a-triumph-im-making-a-note-here-hug")]
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
        [InlineData("longer-than-63-this-was-a-triumph-im-making-a-note-here-huge-suc")]
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

        [Theory]
        [InlineData("really-longer-than-63-this-was-a-triumph-im-making-a-note-here-huge-success")]
        [InlineData("longer-than-63-this-was-a-triumph-im-making-a-note-here-huge-suc")]
        [InlineData("equal-to-63-this-was-a-triumph-im-making-a-note-here-huge-succe")]
        [InlineData("equal-to-63-with-10-this-was-a-triumph-im-making-a-note-here-hu")]
        // 62 car actuellement, on peut aller jusqu'à un nombre de 10
        [InlineData("equal-to-62-this-was-a-triumph-im-making-a-note-here-huge-succ")]
        [InlineData("equal-to-62-with-10-this-was-a-triumph-im-making-a-note-here-h")]
        public async Task GetAvailableSubdomainByPrefixAsync_ShouldCropPrefixWhenTooLong(string prefix)
        {
            var existingDemos = new List<Demo>();
            for(var i = 0; i < SubdomainValidator.MaxDemoPerRequestSubdomain;i++)
            {
                existingDemos.Add(new Demo
                {
                    Subdomain = $"equal-to-63-with-10-this-was-a-triumph-im-making-a-note-here-{i}"
                });
                existingDemos.Add(new Demo
                {
                    Subdomain = $"equal-to-62-with-10-this-was-a-triumph-im-making-a-note-here-{i}"
                });
            }

            _demosStoreMock
                .Setup(ds => ds.GetAsync(It.Is<DemoFilter>(d => d.IsActive == BoolCombination.TrueOnly), It.IsAny<DemoAccess>()))
                .ReturnsAsync(existingDemos);

            var subdomainValidator = new SubdomainValidator(_demosStoreMock.Object, _envStoreMock.Object);
            var availableSubdomain = await subdomainValidator.GetAvailableSubdomainByPrefixAsync(prefix);
            Func<Task> act = async () => await subdomainValidator.ThrowIfInvalidAsync(availableSubdomain);
            await act.Should().NotThrowAsync();
        }

        [Theory]
        [InlineData("a-normal-prefix")]
        // 61 car actuellement, on peut aller jusqu'à un nombre de 10, 61 + 2 = 63 qui est la longueur max
        [InlineData("equal-to-61-this-was-a-triumph-im-making-a-note-here-huge-suc")]
        [InlineData("equal-to-61-with-10-this-was-a-triumph-im-making-a-note-here-")]
        public async Task GetAvailableSubdomainByPrefixAsync_ShouldNotCropPrefixWhenNotNecessary(string prefix)
        {
            var existingDemos = new List<Demo>();
            for (var i = 0; i < SubdomainValidator.MaxDemoPerRequestSubdomain; i++)
            {
                existingDemos.Add(new Demo
                {
                    Subdomain = $"equal-to-61-with-10-this-was-a-triumph-im-making-a-note-here-{i}"
                });
            }
            _demosStoreMock.Setup(ds => ds.GetAsync
                (
                    It.Is<DemoFilter>(d => d.IsActive == BoolCombination.TrueOnly),
                    It.IsAny<DemoAccess>()
                ))
                .ReturnsAsync(existingDemos);

            var subdomainValidator = new SubdomainValidator(_demosStoreMock.Object, _envStoreMock.Object);
            var availableSubdomain = await subdomainValidator.GetAvailableSubdomainByPrefixAsync(prefix);
            Assert.Contains(prefix, availableSubdomain);
            Func<Task> act = async () => await subdomainValidator.ThrowIfInvalidAsync(availableSubdomain);
            await act.Should().NotThrowAsync();
        }
    }
}
