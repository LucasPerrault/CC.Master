using Authentication.Domain;
using Instances.Application.Demos;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Shared;
using Lucca.Core.Rights.Abstractions;
using MockQueryable.Moq;
using Moq;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Application.Tests
{
    public class DemosRepositoryTests
    {
        private readonly Mock<IDemosStore> _demosStoreMock;
        private readonly Mock<IInstancesStore> _instancesStoreMock;
        private readonly Mock<IRightsService> _rightsServiceMock;
        private readonly Mock<ICcDataService> _iCcDataServiceMock;

        public DemosRepositoryTests()
        {
            _demosStoreMock = new Mock<IDemosStore>();
            _instancesStoreMock = new Mock<IInstancesStore>();
            _rightsServiceMock = new Mock<IRightsService>();
            _iCcDataServiceMock = new Mock<ICcDataService>();
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
                new DemoRightsFilter(_rightsServiceMock.Object),
                _iCcDataServiceMock.Object
            );

            _rightsServiceMock
                .Setup(s => s.GetUserOperationHighestScopeAsync(Operation.Demo))
                .ReturnsAsync(Scope.AllDepartments);

            var demos = new List<Demo>()
            {
                new Demo
                {
                    Id = 12,
                    Subdomain = "aperture-science",
                    Instance = new Instance { Id = 123456, IsProtected = false, Cluster = "c1000"}
                }
            };

            _demosStoreMock.Setup(s => s.GetAsync(It.IsAny<DemoFilter>(), It.IsAny<DemoAccess>()))
                .Returns(Task.FromResult(demos.AsQueryable().BuildMock().Object));

            await demosRepo.DeleteAsync(12);

            _demosStoreMock.Verify(s => s.DeleteAsync(It.Is<Demo>(d => d.Id == 12)), Times.Once);
            _instancesStoreMock.Verify(s => s.DeleteForDemoAsync(It.Is<Instance>(i => i.Id == 123456)), Times.Once);
            _iCcDataServiceMock.Verify(s => s.DeleteInstanceAsync
                (
                    It.Is<string>(s => s == "aperture-science"),
                    It.Is<string>(s => s == "c1000"),
                    It.Is<string>(s => string.IsNullOrEmpty(s))
                ), Times.Once);
        }
    }
}
