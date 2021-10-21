using CloudControl.Web.Tests.Mocks;
using FluentAssertions;
using Instances.Application.Instances;
using Instances.Application.Instances.Dtos;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Web.Tests.Controllers
{
    public class HelmControllerTests
    {

        #region GetAsync
        [Theory]
        [InlineData(null, null, null)]
        [InlineData("MyRelease", null, null)]
        [InlineData("MyRelease", null, false)]
        [InlineData("MyRelease", "myBranch", false)]
        public async Task Ok_GetAsync(string releaseName, string gitRef, bool? stable)
        {
            var helmRepositoryMock = new Mock<IHelmRepository>(MockBehavior.Strict);
            helmRepositoryMock
                .Setup(h => h.GetAllReleasesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new List<HelmRelease>());

            var webApplicationFactory = new MockedWebApplicationFactory();
            webApplicationFactory.Mocks.AddSingleton(helmRepositoryMock.Object);

            var httpClient = webApplicationFactory.CreateAuthenticatedClient();

            var query = "";
            var queryJoin = new List<string>();
            if (!string.IsNullOrEmpty(releaseName))
            {
                queryJoin.Add($"releaseName={releaseName}");
            }
            if (!string.IsNullOrEmpty(gitRef))
            {
                queryJoin.Add($"gitRef={gitRef}");
            }
            if (stable != null)
            {
                queryJoin.Add($"lastStable={stable}");
            }
            if (queryJoin.Any())
            {
                query = "?" + string.Join("&", queryJoin);
            }

            var response = await httpClient.GetAsync(
                "/api/helm/releases" + query
            );

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();

            helmRepositoryMock.Verify(h => h.GetAllReleasesAsync(releaseName, gitRef, stable ?? true));
        }


        [Fact]
        public async Task Ko_GetAsync()
        {
            var helmRepositoryMock = new Mock<IHelmRepository>(MockBehavior.Strict);
            helmRepositoryMock
                .Setup(h => h.GetAllReleasesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(new List<HelmRelease>());

            var webApplicationFactory = new MockedWebApplicationFactory();
            webApplicationFactory.Mocks.AddSingleton(helmRepositoryMock.Object);

            var httpClient = webApplicationFactory.CreateAuthenticatedClient();

            var response = await httpClient.GetAsync(
                "/api/helm/releases?gitRef=myBranch"
            );

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();

            helmRepositoryMock.Verify(h => h.GetAllReleasesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }
        #endregion
    }
}
