using Environments.Domain.Storage;
using FluentAssertions;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Demos.Validation;
using Instances.Domain.Instances;
using Instances.Infra.Demos;
using Lucca.Core.Shared.Domain.Exceptions;
using Moq;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
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
        private readonly Mock<IInstanceDuplicationsStore> _duplicationsStoreMock;
        private readonly Mock<ISubdomainValidationTranslator> _subdomainValidationTranslator;

        public SubdomainValidatorTest()
        {
            _demosStoreMock = new Mock<IDemosStore>();
            _envStoreMock = new Mock<IEnvironmentsStore>();
            _duplicationsStoreMock = new Mock<IInstanceDuplicationsStore>();
            _subdomainValidationTranslator = new Mock<ISubdomainValidationTranslator>();
        }

        [Theory]
        [InlineData("aperture-science")]
        [InlineData("Aperture-science")]
        [InlineData("APERTURE-SCIENCE")]
        public async Task IsAvailableAsync_ShouldReturnTrue_WhenSubdomainIsNotTaken(string takenSubdomain)
        {
            _demosStoreMock
                .Setup(s => s.GetAsync(It.Is<DemoFilter>(d => d.IsActive == CompareBoolean.TrueOnly), It.IsAny<AccessRight>()))
                .ReturnsAsync(new List<Demo>());
            _envStoreMock
                .Setup(s => s.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment>());
            _duplicationsStoreMock
                .Setup(s => s.GetPendingForSubdomainAsync(It.Is<string>(d => d == takenSubdomain)))
                .ReturnsAsync(new List<InstanceDuplication>());

            var subdomainValidator = BuildNewSubdomainValidator();

            Assert.True(await subdomainValidator.IsAvailableAsync(takenSubdomain));
        }

        [Theory]
        [InlineData("aperture-science")]
        [InlineData("Aperture-science")]
        [InlineData("APERTURE-SCIENCE")]
        public async Task IsAvailableAsync_ShouldReturnFalse_WhenSubdomainIsTakenByActiveDemo(string takenSubdomain)
        {
            var demos = new List<Demo> { new Demo { Subdomain = takenSubdomain } };

            _demosStoreMock
                .Setup(s => s.GetAsync(It.Is<DemoFilter>(d => d.IsActive == CompareBoolean.TrueOnly), It.IsAny<AccessRight>()))
                .ReturnsAsync(demos);
            _envStoreMock
                .Setup(s => s.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment>());
            _duplicationsStoreMock
                .Setup(s => s.GetPendingForSubdomainAsync(It.Is<string>(d => d == takenSubdomain)))
                .ReturnsAsync(new List<InstanceDuplication>());

            var subdomainValidator = BuildNewSubdomainValidator();

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
                .Setup(s => s.GetAsync(It.Is<DemoFilter>(d => d.IsActive == CompareBoolean.TrueOnly), It.IsAny<AccessRight>()))
                .ReturnsAsync(new List<Demo>());

            _envStoreMock
                .Setup(s => s.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(envs);

            _duplicationsStoreMock
                .Setup(s => s.GetPendingForSubdomainAsync(It.Is<string>(d => d == takenSubdomain)))
                .ReturnsAsync(new List<InstanceDuplication>());

            var subdomainValidator = BuildNewSubdomainValidator();

            Assert.False(await subdomainValidator.IsAvailableAsync("aperture-science"));
        }

        [Theory]
        [InlineData("aperture-science")]
        [InlineData("Aperture-science")]
        [InlineData("APERTURE-SCIENCE")]
        public async Task IsAvailableAsync_ShouldReturnFalse_WhenSubdomainDuplicationIsPending(string takenSubdomain)
        {
            var duplications = new List<InstanceDuplication>
            {
                new InstanceDuplication { TargetSubdomain = takenSubdomain }
            };

            _demosStoreMock
                .Setup(s => s.GetAsync(It.Is<DemoFilter>(d => d.IsActive == CompareBoolean.TrueOnly), It.IsAny<AccessRight>()))
                .ReturnsAsync(new List<Demo>());

            _envStoreMock
                .Setup(s => s.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment>());

            _duplicationsStoreMock
                .Setup(s => s.GetPendingForSubdomainAsync(It.Is<string>(d => d == takenSubdomain)))
                .ReturnsAsync(duplications);

            var subdomainValidator = BuildNewSubdomainValidator();

            Assert.False(await subdomainValidator.IsAvailableAsync(takenSubdomain));
        }

        [Theory]
        [InlineData("aperture-science")]
        [InlineData("not-longer-than-63-this-was-a-triumph-im-making-a-note-here-hug")]
        public async Task IsAvailableAsync_ShouldNotThrow_WhenSubdomainIsNotValid(string validSubdomain)
        {
            var subdomainValidator = BuildNewSubdomainValidator();

            await subdomainValidator.ThrowIfInvalidAsync(validSubdomain);
        }

        [Theory]
        [InlineData("-aperture-science")]
        [InlineData("aperture-science-")]
        [InlineData("aperture#science")]
        [InlineData("aperture-sc??ence")]
        [InlineData("aperture\u1F91science")]
        [InlineData("longer-than-63-this-was-a-triumph-im-making-a-note-here-huge-suc")]
        [InlineData("Aperture-science")]
        [InlineData("APERTURE-SCIENCE")]
        [InlineData("a")]
        [InlineData("cc")]
        [InlineData("iis-aperture")]
        public async Task IsAvailableAsync_ShouldThrow_WhenSubdomainIsNotValid(string invalidSubdomain)
        {
            var subdomainValidator = BuildNewSubdomainValidator();

            await Assert.ThrowsAsync<BadRequestException>(async () => await subdomainValidator.ThrowIfInvalidAsync(invalidSubdomain));
        }

        [Theory]
        [InlineData("really-longer-than-63-this-was-a-triumph-im-making-a-note-here-huge-success")]
        [InlineData("longer-than-63-this-was-a-triumph-im-making-a-note-here-huge-suc")]
        [InlineData("equal-to-63-this-was-a-triumph-im-making-a-note-here-huge-succe")]
        [InlineData("equal-to-63-with-10-this-was-a-triumph-im-making-a-note-here-hu")]
        // 62 car actuellement, on peut aller jusqu'?? un nombre de 10
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
                .Setup(ds => ds.GetAsync(It.Is<DemoFilter>(d => d.IsActive == CompareBoolean.TrueOnly), It.IsAny<AccessRight>()))
                .ReturnsAsync(existingDemos);

            _envStoreMock
                .Setup(s => s.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment>());


            var subdomainValidator = BuildNewSubdomainValidator();
            var availableSubdomain = await subdomainValidator.GetAvailableSubdomainByPrefixAsync(prefix);
            Func<Task> act = async () => await subdomainValidator.ThrowIfInvalidAsync(availableSubdomain);
            await act.Should().NotThrowAsync();
        }

        [Theory]
        [InlineData("a-normal-prefix")]
        // 61 car actuellement, on peut aller jusqu'?? un nombre de 10, 61 + 2 = 63 qui est la longueur max
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
                    It.Is<DemoFilter>(d => d.IsActive == CompareBoolean.TrueOnly),
                    It.IsAny<AccessRight>()
                ))
                .ReturnsAsync(existingDemos);

            _envStoreMock
                .Setup(s => s.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment>());

            var subdomainValidator = BuildNewSubdomainValidator();
            var availableSubdomain = await subdomainValidator.GetAvailableSubdomainByPrefixAsync(prefix);
            Assert.Contains(prefix, availableSubdomain);
            Func<Task> act = async () => await subdomainValidator.ThrowIfInvalidAsync(availableSubdomain);
            await act.Should().NotThrowAsync();
        }

        private SubdomainValidator BuildNewSubdomainValidator()
        {
            return new SubdomainValidator(
                _demosStoreMock.Object,
                _envStoreMock.Object,
                _duplicationsStoreMock.Object,
                _subdomainValidationTranslator.Object
            );
        }
    }
}
