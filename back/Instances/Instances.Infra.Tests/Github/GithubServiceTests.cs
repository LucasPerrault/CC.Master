using FluentAssertions;
using Instances.Infra.Github;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Octokit;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Infra.Tests.Github
{
    public class GithubServiceTests
    {

        private static readonly GitHubClient Client = new GitHubClient(new ProductHeaderValue("mocked"));

        [Fact]
        public async Task ShouldThrowIfUrlIsNotValid()
        {
            var service = new GithubService(new Mock<ILogger<GithubService>>().Object,Client);

            var ex = await Assert.ThrowsAsync<BadRequestException>(() => service.GetFileContentAsync(new Uri("http://github.com/twitter/test"), "path/to/file"));
            ex.Message.Should().Contain("doit commencer par https://github.com");
        }

        [Fact]
        public async Task ShouldThrowIfUrlIsEmpty()
        {
            var service = new GithubService(new Mock<ILogger<GithubService>>().Object,Client);
            await Assert.ThrowsAsync<BadRequestException>(() => service.GetFileContentAsync(null, "path/to/file"));
        }

        [Fact]
        public async Task ShouldThrowIfUrlIsMalformed()
        {
            var service = new GithubService(new Mock<ILogger<GithubService>>().Object,Client);
            var ex = await Assert.ThrowsAsync<BadRequestException>(() => service.GetFileContentAsync(new Uri("https://github.com/himom"), "path/to/file"));
            ex.Message.Should().Contain("doit être de la forme https://github.com/OWNER/REPO");
        }

        [Fact]
        public async Task ShouldThrowIfOwnerIfForbidden()
        {
            var service = new GithubService(new Mock<ILogger<GithubService>>().Object,Client);
            var ex = await Assert.ThrowsAsync<BadRequestException>(() => service.GetFileContentAsync(new Uri("https://github.com/mom/repo"), "path/to/file"));
            ex.Message.Should().Contain("autorisé : LuccaSA");
        }
    }
}
