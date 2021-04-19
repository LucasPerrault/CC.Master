using Instances.Application.Demos;
using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Shared;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Application.Tests
{
    public class DemosRepositoryTests
    {
        private readonly Mock<IDemosStore> _demosStoreMock;
        private readonly Mock<IInstancesStore> _instancesStoreMock;
        private readonly Mock<IDemoRightsFilter> _rightsFilterMock;
        private readonly Mock<ICcDataService> _iCcDataServiceMock;

        public DemosRepositoryTests()
        {
            _demosStoreMock = new Mock<IDemosStore>();
            _instancesStoreMock = new Mock<IInstancesStore>();
            _rightsFilterMock = new Mock<IDemoRightsFilter>();
            _iCcDataServiceMock = new Mock<ICcDataService>();
        }

        [Fact]
        public async Task ShouldDeleteOnLocalAndRemoteAsync()
        {
            var demosRepo = new DemosRepository
            (
                new ClaimsPrincipal(),
                _demosStoreMock.Object,
                _instancesStoreMock.Object,
                _rightsFilterMock.Object,
                _iCcDataServiceMock.Object
            );

            var demos = new List<Demo>()
            {
                new Demo
                {
                    Id = 12,
                    Subdomain = "aperture-science",
                    Instance = new Instance { Id = 123456, IsProtected = false, Cluster = "c1000"}
                }
            };

            _demosStoreMock.Setup(s => s.GetAsync(It.IsAny<Expression<Func<Demo, bool>>[]>()))
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