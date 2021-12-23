using CloudControl.Web.Tests.Mocks;
using CloudControl.Web.Tests.Mocks.Overrides;
using Core.Proxy.Infra.Configuration;
using FluentAssertions;
using IpFilter.Domain;
using IpFilter.Infra.Storage;
using IpFilter.Web;
using Moq;
using Remote.Infra.Extensions;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Testing.Infra;
using Tools;
using Xunit;

namespace IpFilter.Tests.Api;

public class IpFilterControllerTests
{
    public static readonly MockMiddlewareConfig MiddlewareConfig = new MockMiddlewareConfig
    {
        RemoteIpAddress = IPAddress.Parse("123.123.123.123"),
    };

    [Fact]
    public async Task ValidityResponseStatusOk()
    {
        var guid = "deadcafe-f87c-44b6-8f7b-2b58f8cc9e46";
        var dbContext = InMemoryDbHelper.InitialiseDb<IpFilterDbContext>("ip-filter", o => new IpFilterDbContext(o));
        var webApplicationFactory = new MockedWebApplicationFactory();
        webApplicationFactory.Mocks.AddSingleton(MiddlewareConfig);
        webApplicationFactory.Mocks.AddSingleton(new LegacyCloudControlConfiguration { Host = "mocked.legacy" });
        webApplicationFactory.Mocks.AddScoped(dbContext);
        webApplicationFactory.Mocks.AddCustomRegister(s => IpFilterConfigurer.ConfigureServices(s));

        var now = DateTime.Now;
        await dbContext.AddRequestForCurrentUser(Guid.Parse(guid), now, TimeSpan.FromMinutes(42));
        var url = $"/ip-filter/validity?code={guid}";

        var httpClient = webApplicationFactory.CreateAuthenticatedClient();
        var response = await httpClient.GetAsync(url).CatchApplicationErrorBody();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var validity = await Serializer.DeserializeAsync<IpFilterController.RequestValidity>(await response.Content.ReadAsStreamAsync());
        validity.ExpiresAt.Should().Be(now.AddMinutes(42));
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("deadcafe-f87c-44b6-8f7b-2b58f8cc9e46", null)]
    [InlineData(null, "deadcafe-f87c-44b6-8f7b-2b58f8cc9e46")]
    [InlineData("deadcafe-96ca-46a5-afe7-fb437ed2fc3a", "deadcafe-f87c-44b6-8f7b-2b58f8cc9e46")]
    public async Task ValidityResponseStatusNotFound(string remote, string known)
    {
        var dbContext = InMemoryDbHelper.InitialiseDb<IpFilterDbContext>("ip-filter", o => new IpFilterDbContext(o));
        var webApplicationFactory = new MockedWebApplicationFactory();
        webApplicationFactory.Mocks.AddSingleton(MiddlewareConfig);
        webApplicationFactory.Mocks.AddSingleton(new LegacyCloudControlConfiguration { Host = "mocked.legacy" });
        webApplicationFactory.Mocks.AddScoped(dbContext);
        webApplicationFactory.Mocks.AddCustomRegister(s => IpFilterConfigurer.ConfigureServices(s));

        if (known is not null)
        {
            await dbContext.AddRequestForCurrentUser(Guid.Parse(known), DateTime.Now, TimeSpan.FromMinutes(10));
        }

        var url = remote is null
            ? "/ip-filter/validity"
            : $"/ip-filter/validity?code={remote}";

        var httpClient = webApplicationFactory.CreateAuthenticatedClient();
        var response = await httpClient.GetAsync(url).CatchApplicationErrorBody();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RejectionIsOk()
    {
        var guid = Guid.Parse("deadcafe-f87c-44b6-8f7b-2b58f8cc9e46");
        var dbContext = InMemoryDbHelper.InitialiseDb<IpFilterDbContext>("ip-filter", o => new IpFilterDbContext(o));
        var webApplicationFactory = new MockedWebApplicationFactory();
        webApplicationFactory.Mocks.AddSingleton(MiddlewareConfig);
        webApplicationFactory.Mocks.AddSingleton(new LegacyCloudControlConfiguration { Host = "mocked.legacy" });
        webApplicationFactory.Mocks.AddScoped(dbContext);
        webApplicationFactory.Mocks.AddCustomRegister(s => IpFilterConfigurer.ConfigureServices(s));

        var timeMock = new Mock<ITimeProvider>();
        timeMock.Setup(t => t.Now()).Returns(new DateTime(2020, 01, 01, 00, 00, 01));
        webApplicationFactory.Mocks.AddSingleton(timeMock.Object);


        await dbContext.AddRequestForCurrentUser(guid, new DateTime(2020, 01, 01), TimeSpan.FromMinutes(10));
        var request = dbContext.Set<IpFilterAuthorizationRequest>().Single();
        var body = new IpFilterController.RejectionBody { Code = guid }.ToJsonPayload();

        var httpClient = webApplicationFactory.CreateAuthenticatedClient();
        var response = await httpClient.PostAsync("/ip-filter/reject", body).CatchApplicationErrorBody();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        request.RevokedAt.Value.Should().Be(new DateTime(2020, 01, 01, 00, 00, 01));
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("deadcafe-f87c-44b6-8f7b-2b58f8cc9e46", null)]
    [InlineData(null, "deadcafe-f87c-44b6-8f7b-2b58f8cc9e46")]
    [InlineData("deadcafe-96ca-46a5-afe7-fb437ed2fc3a", "deadcafe-f87c-44b6-8f7b-2b58f8cc9e46")]
    public async Task RejectionIsNotFound(string remote, string known)
    {
        var dbContext = InMemoryDbHelper.InitialiseDb<IpFilterDbContext>("ip-filter", o => new IpFilterDbContext(o));
        var webApplicationFactory = new MockedWebApplicationFactory();
        webApplicationFactory.Mocks.AddSingleton(MiddlewareConfig);
        webApplicationFactory.Mocks.AddSingleton(new LegacyCloudControlConfiguration { Host = "mocked.legacy" });
        webApplicationFactory.Mocks.AddScoped(dbContext);
        webApplicationFactory.Mocks.AddCustomRegister(s => IpFilterConfigurer.ConfigureServices(s));

        var timeMock = new Mock<ITimeProvider>();
        timeMock.Setup(t => t.Now()).Returns(new DateTime(2020, 01, 01));
        webApplicationFactory.Mocks.AddSingleton(timeMock.Object);


        if (known is not null)
        {
            await dbContext.AddRequestForCurrentUser(Guid.Parse(known), DateTime.Now, TimeSpan.FromMinutes(10));
        }
        var request = known is not null
            ? dbContext.Set<IpFilterAuthorizationRequest>().Single()
            : null;
        var body = ( remote is not null
            ? new IpFilterController.RejectionBody {Code = Guid.Parse(remote)}
            : null ).ToJsonPayload();

        var httpClient = webApplicationFactory.CreateAuthenticatedClient();
        var response = await httpClient.PostAsync("/ip-filter/reject", body).CatchApplicationErrorBody();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        if (request is not null)
        {
            request.RevokedAt.Should().BeNull();
        }
    }

    [Fact]
    public async Task ValidationIsOk()
    {
        var guid = Guid.Parse("deadcafe-f87c-44b6-8f7b-2b58f8cc9e46");
        var dbContext = InMemoryDbHelper.InitialiseDb<IpFilterDbContext>("ip-filter", o => new IpFilterDbContext(o));
        var webApplicationFactory = new MockedWebApplicationFactory();
        webApplicationFactory.Mocks.AddSingleton(MiddlewareConfig);
        webApplicationFactory.Mocks.AddSingleton(new LegacyCloudControlConfiguration { Host = "mocked.legacy" });
        webApplicationFactory.Mocks.AddScoped(dbContext);
        webApplicationFactory.Mocks.AddCustomRegister(s => IpFilterConfigurer.ConfigureServices(s));

        var timeMock = new Mock<ITimeProvider>();
        timeMock.Setup(t => t.Now()).Returns(new DateTime(2020, 01, 01, 00, 00, 01));
        webApplicationFactory.Mocks.AddSingleton(timeMock.Object);

        await dbContext.AddRequestForCurrentUser(guid, new DateTime(2020, 01, 01), TimeSpan.FromMinutes(10));
        var body = new IpFilterController.ConfirmationBody() { Code = guid, Duration = AuthorizationDuration.OneDay }.ToJsonPayload();

        var httpClient = webApplicationFactory.CreateAuthenticatedClient();
        var response = await httpClient.PostAsync("/ip-filter/confirm", body).CatchApplicationErrorBody();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

public static class TestExtensions
{
    public static async Task AddRequestForCurrentUser(this IpFilterDbContext dbContext, Guid code, DateTime start, TimeSpan duration)
    {
        await dbContext.AddAsync
        (
            new IpFilterAuthorizationRequest
            {
                Code = code,
                CreatedAt = start,
                ExpiresAt = start.Add(duration),
                Address = "123.123.123.123",
                UserId = 0,
            }
        );
        await dbContext.SaveChangesAsync();
    }
}
