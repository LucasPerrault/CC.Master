using Authentication.Domain;
using FluentAssertions;
using IpFilter.Domain;
using IpFilter.Infra.Storage;
using IpFilter.Infra.Storage.Stores;
using IpFilter.Web;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TeamNotification.Abstractions;
using Testing.Infra;
using Tools;
using Users.Domain;
using Xunit;

namespace IpFilter.Tests.Web;

public class IpFilterControllerTests
{
    [Fact]
    public async Task Confirmation_ShouldCreateAuthorization_ForOneDay()
    {
        var guid = Guid.NewGuid();
        var testContext = new ConfirmationTestContext
        {
            Request = new IpFilterAuthorizationRequest
            {
                UserId = 42,
                CreatedAt = new DateTime(2020, 01, 01),
                ExpiresAt = new DateTime(2020, 01, 01, 00, 00, 05),
                Code = guid,
                Address = "123.123.123.123",
            },
            CurrentTime = new DateTime(2020, 01, 01, 00, 00, 01),
            RemoteIp = "123.123.123.123",
            UserId = 42,
            DbContext = InMemoryDbHelper.InitialiseDb<IpFilterDbContext>("ip-filter", o => new IpFilterDbContext(o)),
        };

        var controller = await SetupControllerAsync(testContext);
        await controller.AuthorizeAsync(new IpFilterController.ConfirmationBody { Code = guid, Duration = AuthorizationDuration.OneDay });

        var authorization = testContext.DbContext.Set<IpFilterAuthorization>().Single();
        authorization.IpAddress.Should().Be("123.123.123.123");
        authorization.CreatedAt.Should().Be(new DateTime(2020, 01, 01, 00, 00, 01));
        authorization.ExpiresAt.Should().Be(new DateTime(2020, 01, 02, 00, 00, 01));
        authorization.RequestId.Should().Be(testContext.Request.Id);
        authorization.UserId.Should().Be(42);
    }

    [Fact]
    public async Task Confirmation_ShouldCreateAuthorization_ForSixMonths()
    {
        var guid = Guid.NewGuid();
        var testContext = new ConfirmationTestContext
        {
            Request = new IpFilterAuthorizationRequest
            {
                UserId = 42,
                CreatedAt = new DateTime(2020, 01, 01),
                ExpiresAt = new DateTime(2020, 01, 01, 00, 00, 05),
                Code = guid,
                Address = "123.123.123.123",
            },
            CurrentTime = new DateTime(2020, 01, 01, 00, 00, 01),
            RemoteIp = "123.123.123.123",
            UserId = 42,
            DbContext = InMemoryDbHelper.InitialiseDb<IpFilterDbContext>("ip-filter", o => new IpFilterDbContext(o)),
        };

        var controller = await SetupControllerAsync(testContext);
        await controller.AuthorizeAsync(new IpFilterController.ConfirmationBody { Code = guid, Duration = AuthorizationDuration.SixMonth });

        var authorization = testContext.DbContext.Set<IpFilterAuthorization>().Single();
        authorization.IpAddress.Should().Be("123.123.123.123");
        authorization.CreatedAt.Should().Be(new DateTime(2020, 01, 01, 00, 00, 01));
        authorization.ExpiresAt.Should().Be(new DateTime(2020, 07, 01, 00, 00, 01));
        authorization.RequestId.Should().Be(testContext.Request.Id);
        authorization.UserId.Should().Be(42);
    }

    [Fact]
    public async Task Confirmation_ShouldNotAccept_WhenTooEarly()
    {
        var guid = Guid.NewGuid();
        var testContext = new ConfirmationTestContext
        {
            Request = new IpFilterAuthorizationRequest
            {
                UserId = 42,
                CreatedAt = new DateTime(2020, 01, 01),
                ExpiresAt = new DateTime(2020, 01, 01, 00, 00, 05),
                Code = guid,
                Address = "123.123.123.123",
            },
            CurrentTime = new DateTime(2020, 01, 01),
            RemoteIp = "123.123.123.123",
            UserId = 42,
            DbContext = InMemoryDbHelper.InitialiseDb<IpFilterDbContext>("ip-filter", o => new IpFilterDbContext(o)),
        };

        var controller = await SetupControllerAsync(testContext);
        var exception = await Assert.ThrowsAsync<BadRequestException>
        (
            () => controller.AuthorizeAsync
            (
                new IpFilterController.ConfirmationBody {Code = guid, Duration = AuthorizationDuration.SixMonth}
            )
        );
        exception.Message.Should().Match("*Code is unknown or has expired*");
    }

    [Fact]
    public async Task Confirmation_ShouldNotAccept_WhenTooLate()
    {
        var guid = Guid.NewGuid();
        var testContext = new ConfirmationTestContext
        {
            Request = new IpFilterAuthorizationRequest
            {
                UserId = 42,
                CreatedAt = new DateTime(2020, 01, 01),
                ExpiresAt = new DateTime(2020, 01, 01, 00, 00, 05),
                Code = guid,
                Address = "123.123.123.123",
            },
            CurrentTime = new DateTime(2020, 01, 01, 00, 00, 05),
            RemoteIp = "123.123.123.123",
            UserId = 42,
            DbContext = InMemoryDbHelper.InitialiseDb<IpFilterDbContext>("ip-filter", o => new IpFilterDbContext(o)),
        };

        var controller = await SetupControllerAsync(testContext);
        var exception = await Assert.ThrowsAsync<BadRequestException>
        (
            () => controller.AuthorizeAsync
            (
                new IpFilterController.ConfirmationBody {Code = guid, Duration = AuthorizationDuration.SixMonth}
            )
        );
        exception.Message.Should().Match("*Code is unknown or has expired*");
    }

    [Fact]
    public async Task Confirmation_ShouldNotAccept_WhenNoUserMatch()
    {
        var guid = Guid.NewGuid();
        var testContext = new ConfirmationTestContext
        {
            Request = new IpFilterAuthorizationRequest
            {
                UserId = 44,
                CreatedAt = new DateTime(2020, 01, 01),
                ExpiresAt = new DateTime(2020, 01, 01, 00, 00, 05),
                Code = guid,
                Address = "123.123.123.123",
            },
            CurrentTime = new DateTime(2020, 01, 01, 00, 00, 01),
            RemoteIp = "123.123.123.123",
            UserId = 42,
            DbContext = InMemoryDbHelper.InitialiseDb<IpFilterDbContext>("ip-filter", o => new IpFilterDbContext(o)),
        };

        var controller = await SetupControllerAsync(testContext);
        var exception = await Assert.ThrowsAsync<BadRequestException>
        (
            () => controller.AuthorizeAsync
            (
                new IpFilterController.ConfirmationBody {Code = guid, Duration = AuthorizationDuration.SixMonth}
            )
        );
        exception.Message.Should().Match("*Code is unknown or has expired*");
    }


    [Fact]
    public async Task Confirmation_ShouldNotAccept_WhenNoIpMatch()
    {
        var guid = Guid.NewGuid();
        var testContext = new ConfirmationTestContext
        {
            Request = new IpFilterAuthorizationRequest
            {
                UserId = 42,
                CreatedAt = new DateTime(2020, 01, 01),
                ExpiresAt = new DateTime(2020, 01, 01, 00, 00, 05),
                Code = guid,
                Address = "123.123.123.124",
            },
            CurrentTime = new DateTime(2020, 01, 01, 00, 00, 01),
            RemoteIp = "123.123.123.123",
            UserId = 42,
            DbContext = InMemoryDbHelper.InitialiseDb<IpFilterDbContext>("ip-filter", o => new IpFilterDbContext(o)),
        };

        var controller = await SetupControllerAsync(testContext);
        var exception = await Assert.ThrowsAsync<BadRequestException>
        (
            () => controller.AuthorizeAsync
            (
                new IpFilterController.ConfirmationBody {Code = guid, Duration = AuthorizationDuration.SixMonth}
            )
        );
        exception.Message.Should().Match("*Code is unknown or has expired*");
    }

    [Fact]
    public async Task Confirmation_ShouldNotAccept_WhenAlreadyConsumed()
    {
        var guid = Guid.NewGuid();
        var db = InMemoryDbHelper.InitialiseDb<IpFilterDbContext>("ip-filter", o => new IpFilterDbContext(o));
        var testContext = new ConfirmationTestContext
        {
            Request = new IpFilterAuthorizationRequest
            {
                UserId = 42,
                CreatedAt = new DateTime(2020, 01, 01),
                ExpiresAt = new DateTime(2020, 01, 01, 00, 00, 05),
                Code = guid,
                Address = "123.123.123.123",
            },
            CurrentTime = new DateTime(2020, 01, 01, 00, 00, 01),
            RemoteIp = "123.123.123.123",
            UserId = 42,
            DbContext = db,
        };

        var controller = await SetupControllerAsync(testContext);
        db.Add
        (
            new IpFilterAuthorization
            {
                RequestId = testContext.Request.Id,
                UserId = 0,
                IpAddress = "",
            }
        );
        await db.SaveChangesAsync();
        var exception = await Assert.ThrowsAsync<BadRequestException>
        (
            () => controller.AuthorizeAsync
            (
                new IpFilterController.ConfirmationBody {Code = guid, Duration = AuthorizationDuration.SixMonth}
            )
        );
        exception.Message.Should().Match("*Code has already been used for a prior authorization*");
    }

    private async Task<IpFilterController> SetupControllerAsync(ConfirmationTestContext testContext)
    {
        var principal = new CloudControlUserClaimsPrincipal
        (
            new Principal
            {
                UserId = testContext.UserId,
                User = new User {Id = testContext.UserId },
            }
        );

        testContext.DbContext.Add(testContext.Request);
        await testContext.DbContext.SaveChangesAsync();

        var timeMock = new Mock<ITimeProvider>();
        timeMock.Setup(t => t.Now()).Returns(testContext.CurrentTime);

        var httpContextMock = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext {Connection = {RemoteIpAddress = IPAddress.Parse(testContext.RemoteIp)}};
        httpContextMock.Setup(c => c.HttpContext).Returns(context);

        var ipFilterService = new IpFilterService
        (
            new IpFilterAuthorizationStore(testContext.DbContext),
            new IpFilterAuthorizationRequestStore(testContext.DbContext, timeMock.Object),
            new UserAgentAccessor(httpContextMock.Object),
            timeMock.Object,
            new Mock<ITeamNotifier>().Object
        );
        return new IpFilterController
        (
            new CurrentUserIpAccessor(httpContextMock.Object, principal, ipFilterService),
            ipFilterService,
            principal
        );
    }
}

public class ConfirmationTestContext
{
    public IpFilterAuthorizationRequest Request { get; set; }
    public DateTime CurrentTime { get; set; }
    public int UserId { get; set; }
    public string RemoteIp { get; set; }
    public IpFilterDbContext DbContext { get; set; }
}
