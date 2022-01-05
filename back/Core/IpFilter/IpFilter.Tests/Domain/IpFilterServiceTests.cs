using FluentAssertions;
using IpFilter.Domain;
using IpFilter.Domain.Accessors;
using Lucca.Core.Shared.Domain.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamNotification.Abstractions;
using Tools;
using Xunit;

namespace IpFilter.Tests.Domain
{
    public class IpFilterServiceTests
    {
        private readonly Mock<IIpFilterAuthorizationStore> _authStoreMock = new Mock<IIpFilterAuthorizationStore>(MockBehavior.Strict);
        private readonly Mock<IIpFilterAuthorizationRequestStore> _requestStoreMock = new Mock<IIpFilterAuthorizationRequestStore>(MockBehavior.Strict);
        private readonly Mock<ITimeProvider> _timeMock = new Mock<ITimeProvider>(MockBehavior.Strict);
        private readonly Mock<ITeamNotifier> _teamNotifierMock = new Mock<ITeamNotifier>();
        private readonly Mock<IUserAgentAccessor> _userAgentAccessorMock = new Mock<IUserAgentAccessor>();

        [Fact]
        public async Task GetByUserAsync_ShouldProperlyFilter()
        {
            var now = new DateTime(2020, 01, 01, 20, 00, 00);
            var user = new IpFilterUser { UserId = 123, IpAddress = "127.0.0.1" };

            _timeMock.Setup(t => t.Now()).Returns(now);
            _authStoreMock.Setup(s => s.GetAsync(ItIsAuthorizationFilter(user, now))).ReturnsAsync(new List<IpFilterAuthorization>());

            var service = new IpFilterService( _authStoreMock.Object, _requestStoreMock.Object, _userAgentAccessorMock.Object, _timeMock.Object, _teamNotifierMock.Object);
            await service.GetValidAsync(user);
            _authStoreMock.Verify(s => s.GetAsync(ItIsAuthorizationFilter(user, now)), Times.Once());
        }

        [Fact]
        public async Task AuthorizeAsync_ShouldThrow_WhenExpiredCode()
        {
            var now = new DateTime(2020, 01, 01, 20, 00, 00);
            var user = new IpFilterUser { UserId = 123, IpAddress = "127.0.0.1" };

            _timeMock.Setup(t => t.Now()).Returns(now);

            var service = new IpFilterService( _authStoreMock.Object, _requestStoreMock.Object, _userAgentAccessorMock.Object, _timeMock.Object, _teamNotifierMock.Object);
            var guid = Guid.NewGuid();

            _requestStoreMock.Setup(s => s.FirstOrDefaultAsync(It.IsAny<IpFilterAuthorizationRequestFilter>()))
                .ReturnsAsync((IpFilterAuthorizationRequest)null);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => service.ConfirmAsync(user, guid, AuthorizationDuration.OneDay));
            exception.Message.Should().Match("*Code is unknown or has expired*");
        }

        [Fact]
        public async Task AuthorizeAsync_ShouldCreate_WhenValidCode_ForOneDay()
        {
            var now = new DateTime(2020, 01, 01, 20, 00, 00);
            var user = new IpFilterUser { UserId = 123, IpAddress = "127.0.0.1" };

            _timeMock.Setup(t => t.Now()).Returns(now);

            var service = new IpFilterService( _authStoreMock.Object, _requestStoreMock.Object, _userAgentAccessorMock.Object, _timeMock.Object, _teamNotifierMock.Object);
            var guid = Guid.NewGuid();

            _requestStoreMock.Setup(s => s
                .FirstOrDefaultAsync(It.IsAny<IpFilterAuthorizationRequestFilter>())
            ).ReturnsAsync(new IpFilterAuthorizationRequest { Id = 42 });

            _authStoreMock.Setup(s => s.ExistsAsync(42)).ReturnsAsync(false);
            _authStoreMock
                .Setup
                (
                    s => s.CreateAsync
                    (
                        It.Is<IpFilterAuthorization>
                        (
                            a =>
                                a.RequestId == 42
                                && a.CreatedAt == now
                                && a.IpAddress == "127.0.0.1"
                                && a.UserId == 123
                                && a.ExpiresAt == now.AddDays(1)
                        )
                    )
                ).Returns<IpFilterAuthorization>(Task.FromResult);

            await service.ConfirmAsync(user, guid, AuthorizationDuration.OneDay);
            _authStoreMock.Verify(s =>
                s.CreateAsync(It.IsAny<IpFilterAuthorization>()
                ), Times.Once);
        }

        [Fact]
        public async Task AuthorizeAsync_ShouldCreate_WhenValidCode_ForSixMonth()
        {
            var now = new DateTime(2020, 01, 01, 20, 00, 00);
            var user = new IpFilterUser { UserId = 123, IpAddress = "127.0.0.1" };

            _timeMock.Setup(t => t.Now()).Returns(now);

            var service = new IpFilterService( _authStoreMock.Object, _requestStoreMock.Object, _userAgentAccessorMock.Object, _timeMock.Object, _teamNotifierMock.Object);
            var guid = Guid.NewGuid();

            _requestStoreMock.Setup(s => s
                .FirstOrDefaultAsync(It.IsAny<IpFilterAuthorizationRequestFilter>())
            ).ReturnsAsync(new IpFilterAuthorizationRequest { Id = 42 });

            _authStoreMock.Setup(s => s.ExistsAsync(42)).ReturnsAsync(false);
            _authStoreMock
                .Setup
                (
                    s => s.CreateAsync
                    (
                        It.Is<IpFilterAuthorization>
                        (
                            a =>
                                a.RequestId == 42
                                && a.CreatedAt == now
                                && a.IpAddress == "127.0.0.1"
                                && a.UserId == 123
                                && a.ExpiresAt == now.AddMonths(6)
                        )
                    )
                ).Returns<IpFilterAuthorization>(Task.FromResult);

            await service.ConfirmAsync(user, guid, AuthorizationDuration.SixMonth);
            _authStoreMock.Verify(s =>
                s.CreateAsync(It.IsAny<IpFilterAuthorization>()
                ), Times.Once);
        }

        private IpFilterAuthorizationFilter ItIsAuthorizationFilter(IpFilterUser user, DateTime now)
        {
            return It.Is<IpFilterAuthorizationFilter>
            (
                r => r.UserId == user.UserId
                     && r.IpAddress == user.IpAddress
                     && r.ExpiresAt is IsAfterCompareDateTime
                     && ((IsAfterCompareDateTime)r.ExpiresAt).IsStrict == true
                     && ((IsAfterCompareDateTime)r.ExpiresAt).Value == now
                     && r.CreatedAt is IsBeforeCompareDateTime
                     && ((IsAfterCompareDateTime)r.ExpiresAt).IsStrict == true
                     && ((IsAfterCompareDateTime)r.ExpiresAt).Value == now
            );
        }
    }
}
