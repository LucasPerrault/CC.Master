using CloudControl.Web.Tests.Mocks;
using CloudControl.Web.Tests.Mocks.Overrides;
using Email.Domain;
using FluentAssertions;
using IpFilter.Domain;
using IpFilter.Infra.Storage;
using IpFilter.Web;
using Lucca.Emails.Client.Contracts;
using Moq;
using System;
using System.Net;
using System.Threading.Tasks;
using Testing.Infra;
using Xunit;

namespace IpFilter.Tests.Api;

public class ApiFilterTests
{
    public static readonly IpFilterConfiguration Configuration = new IpFilterConfiguration
    {
        CloudControlBaseAddress = new Uri("https://cc.mocked.url"),
    };

    public static readonly MockMiddlewareConfig MiddlewareConfig = new MockMiddlewareConfig
    {
        RemoteIpAddress = IPAddress.Parse("123.123.123.123"),
    };

    [Fact]
    public async Task ShouldRejectUserWhenIpUnknown()
    {
        var dbContext = InMemoryDbHelper.InitialiseDb<IpFilterDbContext>("ip-filter", o => new IpFilterDbContext(o));
        var webApplicationFactory = new MockedWebApplicationFactory();
        webApplicationFactory.Config.LuccaSecuritySettings.IpWhiteList.ResponseStatusCode = (int) HttpStatusCode.LoopDetected;
        webApplicationFactory.Mocks.AddSingleton(MiddlewareConfig);
        webApplicationFactory.Mocks.AddScoped(dbContext);
        webApplicationFactory.Mocks.AddCustomRegister(s => IpFilterConfigurer.ConfigureServices(s, Configuration));

        var httpClient = webApplicationFactory.CreateAuthenticatedClient();
        var response = await httpClient.GetAsync("/contracts").CatchApplicationErrorBody();
        response.StatusCode.Should().Be(HttpStatusCode.LoopDetected);
    }

    [Fact]
    public async Task ShouldSendEmailWhenIpUnknown()
    {
        var baseAddress = "https://cc.mocked.url";
        var dbContext = InMemoryDbHelper.InitialiseDb<IpFilterDbContext>("ip-filter", o => new IpFilterDbContext(o));
        var webApplicationFactory = new MockedWebApplicationFactory();

        var emailMock = new Mock<IEmailService>();
        var ipFilterEmailsMock = new Mock<IIpFilterEmails>();

        webApplicationFactory.Config.LuccaSecuritySettings.IpWhiteList.ResponseStatusCode = (int) HttpStatusCode.LoopDetected;
        webApplicationFactory.Mocks.AddSingleton(MiddlewareConfig);
        webApplicationFactory.Mocks.AddScoped(dbContext);
        webApplicationFactory.Mocks.AddCustomRegister(s => IpFilterConfigurer.ConfigureServices(s, Configuration));
        webApplicationFactory.Mocks.AddScoped(emailMock.Object);
        webApplicationFactory.Mocks.AddScoped(ipFilterEmailsMock.Object);

        var httpClient = webApplicationFactory.CreateAuthenticatedClient();
        await httpClient.GetAsync("/contracts").CatchApplicationErrorBody();
        emailMock.Verify(e => e.SendAsync(It.IsAny<RecipientForm>(), It.IsAny<EmailContent>()), Times.Once);
        ipFilterEmailsMock.Verify(e => e.GetRejectionEmail(It.IsAny<RejectedUser>(), It.IsAny<IpFilterAuthorizationRequest>(), ItIsHrefBuilder(baseAddress)), Times.Once);
    }

    private static EmailHrefBuilder ItIsHrefBuilder(string baseAddress)
    {
        var guid = new Guid();
        return It.Is<EmailHrefBuilder>(b =>
            b.Accept(guid) == $"{baseAddress}/ip/confirm?code={guid}"
            && b.Reject(guid) == $"{baseAddress}/ip/reject?code={guid}"
        );
    }
}
