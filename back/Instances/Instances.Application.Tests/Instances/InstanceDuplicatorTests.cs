using Instances.Application.CodeSources;
using Instances.Application.Instances;
using Instances.Domain.CodeSources;
using Instances.Domain.Instances;
using Instances.Domain.Shared;
using Instances.Infra.DataDuplication;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Application.Tests
{
    public class InstanceDuplicatorTests
    {
        private readonly Mock<ICcDataService> _ccDataServiceMock;
        private readonly Mock<ICodeSourcesRepository> _codeSourceRepositoryMock;

        public InstanceDuplicatorTests()
        {
            _ccDataServiceMock = new Mock<ICcDataService>();
            _codeSourceRepositoryMock = new Mock<ICodeSourcesRepository>();
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
            _codeSourceRepositoryMock.Setup(csr => csr.GetInstanceCleaningArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());
            _codeSourceRepositoryMock.Setup(csr => csr.GetMonolithArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());

            var instanceDuplicator = new InstancesManipulator(
                new SqlScriptPicker(_codeSourceRepositoryMock.Object),
                _ccDataServiceMock.Object
            );
            await instanceDuplicator.RequestRemoteDuplicationAsync(duplication, withAnonymization: false, skipBufferServer: false, "callback/path");

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
            _codeSourceRepositoryMock.Setup(csr => csr.GetInstanceCleaningArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());
            _codeSourceRepositoryMock.Setup(csr => csr.GetMonolithArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());

            var instanceDuplicator = new InstancesManipulator(
                new SqlScriptPicker(_codeSourceRepositoryMock.Object),
                _ccDataServiceMock.Object);
            await instanceDuplicator.RequestRemoteDuplicationAsync(duplication, withAnonymization: false, skipBufferServer: skipBufferServer, "callback/path");

            _ccDataServiceMock.Verify(ccDataService => ccDataService.StartDuplicateInstanceAsync(It.Is<DuplicateInstanceRequestDto>(dir => dir.SkipBufferServer == skipBufferServer), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

    }
}
