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
            _codeSourceRepositoryMock.Setup(csr => csr.GetInstancePreRestoreArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());
            _codeSourceRepositoryMock.Setup(csr => csr.GetInstancePostRestoreArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());

            var instanceDuplicator = new InstancesManipulator(
                new SqlScriptPicker(_codeSourceRepositoryMock.Object),
                _ccDataServiceMock.Object
            );
            await instanceDuplicator.RequestRemoteDuplicationAsync(duplication, InstanceDuplicationOptions.ForDemo("callback/path"));

            _ccDataServiceMock.Verify(ccDataService => ccDataService.StartDuplicateInstanceAsync(It.IsAny<DuplicateInstanceRequestDto>(), duplication.TargetCluster, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RequestRemoteDuplicationAsync_ShouldIndicateCorrectlyIfWeNeedToSkipTheBufferServerWithTrue()
        {
            var duplication = new InstanceDuplication
            {
                SourceCluster = "demo1",
                TargetCluster = "demo2",
            };
            var duplicationOptions = InstanceDuplicationOptions.ForDemo("callback/path");
            Assert.True(duplicationOptions.SkipBufferServer);
            _ccDataServiceMock.Setup(ccDataService => ccDataService.StartDuplicateInstanceAsync(It.IsAny<DuplicateInstanceRequestDto>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            _codeSourceRepositoryMock.Setup(csr => csr.GetInstanceCleaningArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());
            _codeSourceRepositoryMock.Setup(csr => csr.GetMonolithArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());
            _codeSourceRepositoryMock.Setup(csr => csr.GetInstancePreRestoreArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());
            _codeSourceRepositoryMock.Setup(csr => csr.GetInstancePostRestoreArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());

            var instanceDuplicator = new InstancesManipulator(
                new SqlScriptPicker(_codeSourceRepositoryMock.Object),
                _ccDataServiceMock.Object);
            await instanceDuplicator.RequestRemoteDuplicationAsync(duplication, duplicationOptions);

            _ccDataServiceMock.Verify(ccDataService => ccDataService.StartDuplicateInstanceAsync(It.Is<DuplicateInstanceRequestDto>(dir => dir.SkipBufferServer == duplicationOptions.SkipBufferServer), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RequestRemoteDuplicationAsync_ShouldIndicateCorrectlyIfWeNeedToSkipTheBufferServerWithFalse()
        {
            var duplication = new InstanceDuplication
            {
                SourceCluster = "cluster1",
                TargetCluster = "formation",
            };
            var duplicationOptions = InstanceDuplicationOptions.ForTraining(withAnonymization: false, keepExistingPasswords: false, callBackPath: "callback/path");
            Assert.False(duplicationOptions.SkipBufferServer);
            _ccDataServiceMock.Setup(ccDataService => ccDataService.StartDuplicateInstanceAsync(It.IsAny<DuplicateInstanceRequestDto>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            _codeSourceRepositoryMock.Setup(csr => csr.GetInstanceCleaningArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());
            _codeSourceRepositoryMock.Setup(csr => csr.GetMonolithArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());
            _codeSourceRepositoryMock.Setup(csr => csr.GetInstancePreRestoreArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());
            _codeSourceRepositoryMock.Setup(csr => csr.GetInstancePostRestoreArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());

            var instanceDuplicator = new InstancesManipulator(
                new SqlScriptPicker(_codeSourceRepositoryMock.Object),
                _ccDataServiceMock.Object);
            await instanceDuplicator.RequestRemoteDuplicationAsync(duplication, duplicationOptions);

            _ccDataServiceMock.Verify(ccDataService => ccDataService.StartDuplicateInstanceAsync(It.Is<DuplicateInstanceRequestDto>(dir => dir.SkipBufferServer == duplicationOptions.SkipBufferServer), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

    }
}
