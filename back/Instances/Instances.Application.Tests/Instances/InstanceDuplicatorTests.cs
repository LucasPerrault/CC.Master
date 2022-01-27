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

            var instanceDuplicator = new InstancesDuplicator(
                new SqlScriptPicker(new SqlScriptPickerConfiguration
                {
                    JenkinsBaseUri = new Uri("http://localhost/"),
                    MonolithJobPath = "MO-NO-LI-TH"
                }),
                _ccDataServiceMock.Object);
            await instanceDuplicator.RequestRemoteDuplicationAsync(duplication, skipBufferServer: true, "callback/path");

            _ccDataServiceMock.Verify(ccDataService => ccDataService.StartDuplicateInstanceAsync(It.IsAny<DuplicateInstanceRequestDto>(), duplication.TargetCluster, It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task RequestRemoteDuplicationAsync_ShouldIndicateCorrectlyIfWeNeedToSkipTheBufferServer(bool skipBufferServer)
        {
            var duplication = new InstanceDuplication
            {
                SourceCluster = "demo1",
                TargetCluster = "demo2",
            };
            _ccDataServiceMock.Setup(ccDataService => ccDataService.StartDuplicateInstanceAsync(It.IsAny<DuplicateInstanceRequestDto>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            var instanceDuplicator = new InstancesDuplicator(
                new SqlScriptPicker(new SqlScriptPickerConfiguration
                {
                    JenkinsBaseUri = new Uri("http://localhost/"),
                    MonolithJobPath = "MO-NO-LI-TH"
                }),
                _ccDataServiceMock.Object);
            await instanceDuplicator.RequestRemoteDuplicationAsync(duplication, skipBufferServer: skipBufferServer, "callback/path");

            _ccDataServiceMock.Verify(ccDataService => ccDataService.StartDuplicateInstanceAsync(It.Is<DuplicateInstanceRequestDto>(dir => dir.SkipBufferServer == skipBufferServer), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

    }
}
