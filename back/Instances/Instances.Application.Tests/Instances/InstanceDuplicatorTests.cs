using Instances.Application.Instances;
using Instances.Domain.Instances;
using Instances.Domain.Shared;
using Instances.Infra.DataDuplication;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Application.Tests
{
    public class InstanceDuplicatorTests
    {
        private readonly Mock<ICcDataService> _ccDataServiceMock;

        public InstanceDuplicatorTests()
        {
            _ccDataServiceMock = new Mock<ICcDataService>();
        }

        [Fact]
        public async Task RequestRemoteDuplicationAsync_ShouldCallTheCCDataOfTheTargetClusterAsync()
        {
            var duplication = new InstanceDuplication
            {
                SourceCluster = "demo1",
                TargetCluster = "demo2",
            };
            _ccDataServiceMock.Setup(ccDataService => ccDataService.StartDuplicateInstanceAsync(It.IsAny<DuplicateInstanceRequestDto>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            var instanceDuplicator = new InstancesManipulator(
                new SqlScriptPicker(new SqlScriptPickerConfiguration
                {
                    JenkinsBaseUri = new Uri("http://localhost/"),
                    MonolithJobPath = "MO-NO-LI-TH"
                }),
                _ccDataServiceMock.Object);
            await instanceDuplicator.RequestRemoteDuplicationAsync(duplication, "callback/path");

            _ccDataServiceMock.Verify(ccDataService => ccDataService.StartDuplicateInstanceAsync(It.IsAny<DuplicateInstanceRequestDto>(), duplication.TargetCluster, It.IsAny<string>()), Times.Once);
        }
    }
}
