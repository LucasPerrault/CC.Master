using Authentication.Domain;
using Environments.Domain;
using Environments.Domain.ExtensionInterface;
using Environments.Domain.Storage;
using FluentAssertions;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Slack.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Environment = Environments.Domain.Environment;

namespace Environments.Application.Tests
{
    public class EnvironmentRenamingServiceTests
    {
        private readonly Mock<IEnvironmentsStore> _environmentStoreMock;
        private readonly Mock<IEnvironmentsRenamingStore> _environmentRenamingStoreMock;
        private readonly Mock<IEnvironmentRenamingExtension> _environmentRenamingExtensionMock;
        private readonly Mock<ISlackClient> _slackClientMock;
        private readonly EnvironmentRenamingService _environmentRenamingService;

        public EnvironmentRenamingServiceTests()
        {
            _environmentStoreMock = new Mock<IEnvironmentsStore>(MockBehavior.Strict);
            _environmentRenamingStoreMock = new Mock<IEnvironmentsRenamingStore>(MockBehavior.Strict);
            _environmentRenamingExtensionMock = new Mock<IEnvironmentRenamingExtension>(MockBehavior.Strict);
            _environmentRenamingExtensionMock.Setup(s => s.ExtensionName).Returns("extensionName");
            _slackClientMock = new Mock<ISlackClient>(MockBehavior.Strict);
            _slackClientMock
                .Setup(c => c.SendMessageAsync(It.IsAny<string>(), It.IsAny<SlackMessage>()))
                .ReturnsAsync(new Mock<ISlackMessageLifeManager>().Object);
            _environmentRenamingService = new EnvironmentRenamingService(
                _environmentStoreMock.Object,
                _environmentRenamingStoreMock.Object,
                new List<IEnvironmentRenamingExtension> { _environmentRenamingExtensionMock.Object },
                new CloudControlUserClaimsPrincipal(new Principal
                {
                    UserId = 42,
                    User = new Users.Domain.User()
                }),
                _slackClientMock.Object,
                new EnvironmentRenamingConfiguration { SlackChannel = "channel" },
                new Mock<ILogger<EnvironmentRenamingService>>().Object
            );
        }

        #region RenameAsync
        [Fact]
        public async Task RenameAsync_Ok()
        {
            var environmentId = 2;
            var environment = new Environment { Id = environmentId };
            _environmentStoreMock
                .Setup(e => e.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment> { environment });
            _environmentStoreMock
                .Setup(e => e.UpdateSubDomainAsync(It.IsAny<Environment>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            EnvironmentRenaming capturedEnvironmentRenaming = null;
            _environmentRenamingStoreMock
                .Setup(e => e.CreateAsync(It.IsAny<EnvironmentRenaming>()))
                    .Returns((EnvironmentRenaming e) =>
                    {
                        capturedEnvironmentRenaming = e;
                        return Task.FromResult(e);
                    });
            _environmentRenamingExtensionMock
                .Setup(e => e.RenameAsync(It.IsAny<Environment>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var status = await _environmentRenamingService.RenameAsync(2, "newName");

            _environmentStoreMock.Verify(e => e.GetAsync(EnvironmentAccessRight.Everything, It.Is<EnvironmentFilter>(f => f.Ids.Contains(2))));
            _environmentStoreMock.Verify(e => e.UpdateSubDomainAsync(environment, "newName"));
            _environmentRenamingExtensionMock.Verify(e => e.RenameAsync(environment, "newName"));
            status.Status.Should().Be(EnvironmentRenamingStatus.SUCCESS);
            capturedEnvironmentRenaming.Should().NotBeNull();
            capturedEnvironmentRenaming.UserId.Should().Be(42);
            capturedEnvironmentRenaming.Status.Should().Be(EnvironmentRenamingStatus.SUCCESS);
            capturedEnvironmentRenaming.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public async Task RenameAsync_ExtensionError()
        {
            var environmentId = 2;
            var environment = new Environment { Id = environmentId };
            _environmentStoreMock
                .Setup(e => e.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment> { environment });
            _environmentStoreMock
                .Setup(e => e.UpdateSubDomainAsync(It.IsAny<Environment>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            EnvironmentRenaming capturedEnvironmentRenaming = null;
            _environmentRenamingStoreMock
                .Setup(e => e.CreateAsync(It.IsAny<EnvironmentRenaming>()))
                    .Returns((EnvironmentRenaming e) =>
                    {
                        capturedEnvironmentRenaming = e;
                        return Task.FromResult(e);
                    });
            _environmentRenamingExtensionMock
                .Setup(e => e.RenameAsync(It.IsAny<Environment>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("test"));

            var status = await _environmentRenamingService.RenameAsync(2, "newName");

            _environmentStoreMock.Verify(e => e.GetAsync(EnvironmentAccessRight.Everything, It.Is<EnvironmentFilter>(f => f.Ids.Contains(2))));
            _environmentStoreMock.Verify(e => e.UpdateSubDomainAsync(environment, "newName"));
            _environmentRenamingExtensionMock.Verify(e => e.RenameAsync(environment, "newName"));
            status.Status.Should().Be(EnvironmentRenamingStatus.ERROR);
            capturedEnvironmentRenaming.Should().NotBeNull();
            capturedEnvironmentRenaming.UserId.Should().Be(42);
            capturedEnvironmentRenaming.Status.Should().Be(EnvironmentRenamingStatus.ERROR);
            capturedEnvironmentRenaming.ErrorMessage.Should().Be("test");
        }

        [Fact]
        public async Task RenameAsync_NotFound()
        {
            var environmentId = 2;
            var environment = new Environment { Id = environmentId };
            _environmentStoreMock
                .Setup(e => e.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment>());

            Func<Task> act = () => _environmentRenamingService.RenameAsync(2, "newName");

            (await act.Should().ThrowAsync<DomainException>())
                .And.Status.Should().Be(DomainExceptionCode.BadRequest);
        }

        #endregion
    }
}
