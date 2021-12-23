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
    public static readonly MockMiddlewareConfig MiddlewareConfig = new MockMiddlewareConfig
    {
        RemoteIpAddress = IPAddress.Parse("123.123.123.123"),
    };

    [Fact]
    public async Task ShouldRejectUserWhenIpUnknown()
    {
        var dbContext = InMemoryDbHelper.InitialiseDb<IpFilterDbContext>("ip-filter", o => new IpFilterDbContext(o));
        var webApplicationFactory = new MockedWebApplicationFactory();
        webApplicationFactory.Mocks.AddSingleton(MiddlewareConfig);
        webApplicationFactory.Mocks.AddScoped(dbContext);
        webApplicationFactory.Mocks.AddCustomRegister(s => IpFilterConfigurer.ConfigureServices(s));

        var httpClient = webApplicationFactory.CreateAuthenticatedClient();
        var response = await httpClient.GetAsync("/contracts").CatchApplicationErrorBody();
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location.Should().Be("https://localhost/ip");
    }

    [Fact]
    public async Task ShouldSendEmailWhenIpUnknown()
    {
        var dbContext = InMemoryDbHelper.InitialiseDb<IpFilterDbContext>("ip-filter", o => new IpFilterDbContext(o));
        var webApplicationFactory = new MockedWebApplicationFactory();

        var emailMock = new Mock<IEmailService>();
        var ipFilterEmailsMock = new Mock<IIpFilterEmails>();

        webApplicationFactory.Mocks.AddSingleton(MiddlewareConfig);
        webApplicationFactory.Mocks.AddScoped(dbContext);
        webApplicationFactory.Mocks.AddCustomRegister(s => IpFilterConfigurer.ConfigureServices(s));
        webApplicationFactory.Mocks.AddScoped(emailMock.Object);
        webApplicationFactory.Mocks.AddScoped(ipFilterEmailsMock.Object);

        var httpClient = webApplicationFactory.CreateAuthenticatedClient();
        await httpClient.GetAsync("/contracts").CatchApplicationErrorBody();
        emailMock.Verify(e => e.SendAsync(It.IsAny<RecipientForm>(), It.IsAny<EmailContent>()), Times.Once);
        ipFilterEmailsMock.Verify(e => e.GetRejectionEmail(It.IsAny<RejectedUser>(), It.IsAny<IpFilterAuthorizationRequest>(), ItIsHrefBuilder()), Times.Once);
    }

    private static EmailHrefBuilder ItIsHrefBuilder()
    {
        var guid = new Guid();
        return It.Is<EmailHrefBuilder>(b =>
            b.Accept(guid) == $"https://localhost/ip/confirm?code={guid}&redirection=%2fcontracts"
            && b.Reject(guid) == $"https://localhost/ip/reject?code={guid}"
        );
    }
}
