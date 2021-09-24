using Authentication.Domain;
using Instances.Application.Demos;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Shared;
using Moq;
using Rights.Domain;
using Rights.Domain.Abstractions;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Application.Tests.Demos
{
    public class DemosRepositoryTests
    {
        private readonly Mock<IDemosStore> _demosStoreMock;
        private readonly Mock<IInstancesStore> _instancesStoreMock;
        private readonly Mock<IRightsService> _rightsServiceMock;
        private readonly Mock<ICcDataService> _iCcDataServiceMock;
        private readonly Mock<IDnsService> _dnsServiceMock;

        public DemosRepositoryTests()
        {
            _demosStoreMock = new Mock<IDemosStore>();
            _instancesStoreMock = new Mock<IInstancesStore>();
            _rightsServiceMock = new Mock<IRightsService>();
            _iCcDataServiceMock = new Mock<ICcDataService>();
            _dnsServiceMock = new Mock<IDnsService>();
        }

        [Fact]
        public async Task ShouldDeleteOnLocalAndRemoteAsync()
        {
            var claimsPrincipal = new CloudControlApiKeyClaimsPrincipal(new ApiKey
            {
                Name = "Mocked Api Key",
                Token = Guid.NewGuid()
            });

            var demosRepo = new DemosRepository(
                claimsPrincipal,
                _demosStoreMock.Object,
                _instancesStoreMock.Object,
                new DemoRightsFilter(new RightsFilter(_rightsServiceMock.Object)),
                _iCcDataServiceMock.Object,
                _dnsServiceMock.Object
            );

            _rightsServiceMock
                .Setup(s => s.GetUserOperationHighestScopeAsync(Operation.Demo))
                .ReturnsAsync(AccessRightScope.AllDistributors);

            var demo = new Demo
            {
                Id = 12,
                Subdomain = "aperture-science",
                Cluster = "c1000",
                Instance = new Instance { Id = 123456, IsProtected = false }
            };
            var demos = new List<Demo> { demo };

            _demosStoreMock.Setup(s => s.GetActiveByIdAsync(It.IsAny<int>(), It.IsAny<AccessRight>()))
                .ReturnsAsync(demo);

            await demosRepo.DeleteAsync(12);

            _demosStoreMock.Verify(s => s.DeleteAsync(It.Is<Demo>(d => d.Id == 12)), Times.Once);
            _instancesStoreMock.Verify(s => s.DeleteAsync(It.Is<Instance>(i => i.Id == 123456)), Times.Once);
            _iCcDataServiceMock.Verify(s => s.DeleteInstanceAsync
                (
                    It.Is<string>(s => s == "aperture-science"),
                    It.Is<string>(s => s == "c1000"),
                    It.Is<string>(s => string.IsNullOrEmpty(s))
                ), Times.Once);
        }
    }
}
