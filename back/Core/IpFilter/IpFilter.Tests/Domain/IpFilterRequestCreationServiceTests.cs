using Email.Domain;
using IpFilter.Domain;
using Lock;
using Lucca.Emails.Client.Contracts;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;
using Xunit;

namespace IpFilter.Tests.Domain;

public class IpFilterRequestCreationServiceTests
{
    private readonly Mock<IEmailService> _emailService = new Mock<IEmailService>();
    private readonly Mock<IIpFilterEmails> _ipFilterEmailsMock = new Mock<IIpFilterEmails>();
    private readonly Mock<ILockService> _lockMock = new Mock<ILockService>();
    private readonly Mock<IIpFilterAuthorizationRequestStore> _storeMock = new Mock<IIpFilterAuthorizationRequestStore>(MockBehavior.Strict);
    private readonly Mock<ITimeProvider> _timeMock = new Mock<ITimeProvider>();
    private readonly Mock<IGuidGenerator> _guidMock = new Mock<IGuidGenerator>();

    [Fact]
    public async Task ShouldNotifyUserOnIpRejection()
    {
        var now = new DateTime(2020, 01, 01);
        SetupCurrentDate(now);

        var user = new RejectedUser
        {
            FirstName = "Chell",
            LastName = "XXXXXX",
            IpFilterUser = new IpFilterUser {UserId = 42, IpAddress = "123.0.0.0"},
        };

        _storeMock.Setup(s => s.GetAsync(It.IsAny<IpFilterAuthorizationRequestFilter>()))
            .ReturnsAsync(new List<IpFilterAuthorizationRequest>());
        _storeMock.Setup(s => s.CreateAsync(Matching(42, "123.0.0.0", now, now.AddMinutes(10))))
            .Returns<IpFilterAuthorizationRequest>(Task.FromResult);

        var service = NewServiceInstance();
        await service.SendRequestIfNeededAsync(user, new EmailHrefBuilder());
        _storeMock.Verify(s => s.CreateAsync(Matching(42, "123.0.0.0", now, now.AddMinutes(10))), Times.Once);
        _emailService.Verify(e => e.SendAsync(It.Is<RecipientForm>(f => f.UserId == 42), It.IsAny<EmailContent>()), Times.Once);
    }

    [Fact]
    public async Task ShouldPreventEmailSpam()
    {
        var now = new DateTime(2020, 01, 01);
        SetupCurrentDate(now);

        var user = new RejectedUser
        {
            FirstName = "Chell",
            LastName = "XXXXXX",
            IpFilterUser = new IpFilterUser {UserId = 42, IpAddress = "123.0.0.0"},
        };

        _storeMock.Setup(s => s.GetAsync(It.IsAny<IpFilterAuthorizationRequestFilter>()))
            .ReturnsAsync(new List<IpFilterAuthorizationRequest>());
        _storeMock.Setup(s => s.CreateAsync(Matching(42, "123.0.0.0", now, now.AddMinutes(10))))
            .Returns<IpFilterAuthorizationRequest>(Task.FromResult);

        var service = NewServiceInstance();
        await service.SendRequestIfNeededAsync(user, new EmailHrefBuilder());
        await service.SendRequestIfNeededAsync(user, new EmailHrefBuilder());
        _emailService.Verify(e => e.SendAsync(It.Is<RecipientForm>(f => f.UserId == 42), It.IsAny<EmailContent>()), Times.Once);
        _ipFilterEmailsMock.Verify(e => e.GetRejectionEmail(It.Is<RejectedUser>(u =>u.FirstName == "Chell" && u.LastName == "XXXXXX"), It.IsAny<IpFilterAuthorizationRequest>(), It.IsAny<EmailHrefBuilder>()), Times.Once);
    }

    [Fact]
    public async Task ShouldSendEmailForExistingRequest()
    {
        var now = new DateTime(2020, 01, 01);
        SetupCurrentDate(now);

        var user = new RejectedUser
        {
            FirstName = "Chell",
            LastName = "XXXXXX",
            IpFilterUser = new IpFilterUser {UserId = 42, IpAddress = "123.0.0.0"},
        };

        _storeMock.Setup(s => s.GetAsync(It.IsAny<IpFilterAuthorizationRequestFilter>()))
            .ReturnsAsync(new List<IpFilterAuthorizationRequest>
            {
                new IpFilterAuthorizationRequest
                {
                    UserId = 42,
                    Address = "123.0.0.0",
                    CreatedAt = now.Subtract(TimeSpan.FromMilliseconds(10)),
                    ExpiresAt = now.Add(TimeSpan.FromMinutes(40)),
                },
            });

        var service = NewServiceInstance();
        await service.SendRequestIfNeededAsync(user, new EmailHrefBuilder());
        await service.SendRequestIfNeededAsync(user, new EmailHrefBuilder());
        _emailService.Verify(e => e.SendAsync(It.Is<RecipientForm>(f => f.UserId == 42), It.IsAny<EmailContent>()), Times.Once);
    }


        [Fact]
        public async Task CreateRequestIfNonePendingAsync_ShouldCreateWhenNonePending()
        {
            var now = new DateTime(2020, 01, 01, 20, 00, 00);
            var user = new IpFilterUser { UserId = 123, IpAddress = "127.0.0.1" };

            _timeMock.Setup(t => t.Now()).Returns(now);
            _storeMock
                .Setup(r => r.GetAsync(It.IsAny<IpFilterAuthorizationRequestFilter>()))
                .ReturnsAsync(new List<IpFilterAuthorizationRequest>());
            _storeMock
                .Setup(r => r.CreateAsync(It.IsAny<IpFilterAuthorizationRequest>()))
                .ReturnsAsync(new IpFilterAuthorizationRequest());

            var service = NewServiceInstance();
            await service.CreateOrGetPendingAsync(user);

            _storeMock.Verify(s => s.CreateAsync(It.Is<IpFilterAuthorizationRequest>(r =>
                r.Address == user.IpAddress && r.UserId == user.UserId)), Times.Once);
        }

        [Fact]
        public async Task CreateRequestIfNonePendingAsync_ShouldNotCreateWhenHasPending()
        {
            var now = new DateTime(2020, 01, 01, 20, 00, 00);
            var user = new IpFilterUser { UserId = 123, IpAddress = "127.0.0.1" };

            _timeMock.Setup(t => t.Now()).Returns(now);
            _storeMock
                .Setup(r => r.GetAsync(ItIsRequestFilter(user, now)))
                .ReturnsAsync(new List<IpFilterAuthorizationRequest> { new IpFilterAuthorizationRequest() });

            _storeMock
                .Setup(r => r.CreateAsync(It.IsAny<IpFilterAuthorizationRequest>()))
                .ReturnsAsync(new IpFilterAuthorizationRequest());

            var service = NewServiceInstance();
            await service.CreateOrGetPendingAsync(user);
            _storeMock.Verify(s => s.CreateAsync(It.IsAny<IpFilterAuthorizationRequest>()), Times.Never);
        }

    private void SetupCurrentDate(DateTime dateTime)
    {
        _timeMock.Setup(t => t.Now()).Returns(dateTime);
        _timeMock.Setup(t => t.Today()).Returns(dateTime);
    }

    private IpFilterRequestCreationService NewServiceInstance()
    {
        return new IpFilterRequestCreationService
        (
            _emailService.Object,
            _ipFilterEmailsMock.Object,
            _lockMock.Object,
            _storeMock.Object,
            _timeMock.Object,
            _guidMock.Object
        );
    }

    private IpFilterAuthorizationRequest Matching(int userId, string address, DateTime now, DateTime expiration)
    {
        return It.Is<IpFilterAuthorizationRequest>
        (
            r =>
                r.Address.Equals(address, StringComparison.OrdinalIgnoreCase)
                && r.UserId == userId
                && r.CreatedAt == now
                && r.ExpiresAt == expiration
        );
    }

    private IpFilterAuthorizationRequestFilter ItIsRequestFilter(IpFilterUser user, DateTime now)
    {
        return It.Is<IpFilterAuthorizationRequestFilter>
        (
            r => r.UserId == user.UserId
                 && r.Addresses.Contains(user.IpAddress)
                 && r.ExpiresAt is IsAfterCompareDateTime
                 && ((IsAfterCompareDateTime)r.ExpiresAt).IsStrict == true
                 && ((IsAfterCompareDateTime)r.ExpiresAt).Value == now.AddMinutes(10)
                 && r.RevokedAt is IsNullCompareNullableDateTime
        );
    }
}
