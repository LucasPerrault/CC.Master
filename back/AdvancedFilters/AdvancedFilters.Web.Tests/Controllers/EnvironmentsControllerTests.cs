using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Services;
using CloudControl.Web.Tests.Mocks;
using FluentAssertions;
using Lucca.Core.Api.Abstractions.Paging;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Xunit;

namespace AdvancedFilters.Web.Tests.Controllers
{
    public class EnvironmentsControllerTests
    {
        private const string _endpointBase = "/api/cafe/environments";

        #region GetAsync
        [Fact]
        public async Task GetAsync_Ok()
        {
            var envStoreMock = new Mock<IEnvironmentsStore>(MockBehavior.Strict);
            envStoreMock
                .Setup(s => s.GetAsync(It.IsAny<IPageToken>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new Page<Environment>
                {
                    Items = new List<Environment> { new Environment() }
                });

            var webApplicationFactory = new MockedWebApplicationFactory();

            var exportServiceMock = new Mock<IExportService>();
            webApplicationFactory.Mocks.AddScoped(envStoreMock.Object);
            webApplicationFactory.Mocks.AddScoped(exportServiceMock.Object);

            var httpClient = webApplicationFactory.CreateAuthenticatedClient();

            var response = await httpClient.GetAsync(_endpointBase);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await Serializer.DeserializeAsync<Page<Environment>>(await response.Content.ReadAsStreamAsync());
            content.Items.Should().HaveCount(1);

            envStoreMock.Verify(s => s.GetAsync(It.IsAny<IPageToken>(), It.IsAny<EnvironmentFilter>()));
        }
        #endregion

        #region SearchAsync
        [Fact]
        public async Task SearchAsync_Ok()
        {
            var envStoreMock = new Mock<IEnvironmentsStore>(MockBehavior.Strict);
            envStoreMock
                .Setup(s => s.SearchAsync(It.IsAny<IPageToken>(), It.IsAny<IAdvancedFilter>()))
                .ReturnsAsync(new Page<Environment>
                {
                    Items = new List<Environment> { new Environment() }
                });

            var exportServiceMock = new Mock<IExportService>();

            var webApplicationFactory = new MockedWebApplicationFactory();
            webApplicationFactory.Mocks.AddScoped(envStoreMock.Object);
            webApplicationFactory.Mocks.AddScoped(exportServiceMock.Object);

            var httpClient = webApplicationFactory.CreateAuthenticatedClient();

            var filter = new EnvironmentAdvancedCriterion();
            var json = Serializer.Serialize(filter);
            var stringContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await httpClient.PostAsync($"{_endpointBase}/search", stringContent);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await Serializer.DeserializeAsync<Page<Environment>>(await response.Content.ReadAsStreamAsync());
            content.Items.Should().HaveCount(1);

            envStoreMock.Verify(s => s.SearchAsync(It.IsAny<IPageToken>(), It.IsAny<IAdvancedFilter>()));
        }
        #endregion
    }
}
