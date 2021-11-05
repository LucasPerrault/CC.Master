using Authentication.Domain;
using Authentication.Infra.Services;
using Authentication.Infra.Storage;
using Lucca.Core.Shared.Domain.Contracts.Principals;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Users.Domain;
using Xunit;

namespace Authentication.Infra.Tests
{
    public class PrincipalStoreTests
    {
        private readonly Mock<IUserAuthenticationRemoteService> _userServiceMock;
        private readonly Mock<IApiKeyAuthenticationRemoteService> _apiKeyServiceMock;
        private readonly Mock<ILogger<PrincipalStore>> _loggerMock;

        public PrincipalStoreTests()
        {
            _userServiceMock = new Mock<IUserAuthenticationRemoteService>();
            _apiKeyServiceMock = new Mock<IApiKeyAuthenticationRemoteService>();
            _loggerMock = new Mock<ILogger<PrincipalStore>>();
        }

        [Fact]
        public async Task ShouldReturnClaimsPrincipal()
        {
            var store = new PrincipalStore(_userServiceMock.Object, _apiKeyServiceMock.Object, _loggerMock.Object);
            var guid = Guid.NewGuid();
            _userServiceMock
                .Setup(s => s.GetUserPrincipalAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(new Principal
            {
                Token = guid,
                User = new User { Id = 123 },
                UserId = 123
            }));
            var user = await store.GetPrincipalAsync(PrincipalType.User, guid);
            Assert.NotNull(user);
            Assert.True(user is CloudControlUserClaimsPrincipal);
            Assert.Equal(guid, ((CloudControlUserClaimsPrincipal)user).Token);
        }

        [Fact]
        public async Task ShouldSilentlyFailOnPartenairesFail()
        {
            var store = new PrincipalStore(_userServiceMock.Object, _apiKeyServiceMock.Object, _loggerMock.Object);
            _userServiceMock.Setup(s => s.GetUserPrincipalAsync(It.IsAny<Guid>())).Throws(new Exception());
            var user = await store.GetPrincipalAsync(PrincipalType.User, Guid.NewGuid());
            Assert.Null(user);
        }
    }
}
