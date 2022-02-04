using Instances.Application.Instances;
using Instances.Application.Webhooks.Github;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Moq;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Application.Tests.Webhooks.Github
{
    public class PushWebhookServiceTests
    {
        private readonly Uri _repositoryUrl = new("https://github.com/LuccaSA/repo");
        private readonly PushWebhookService _pushWebhookService;
        private readonly Mock<IGithubBranchesRepository> _githubBranchesRepositoryMock;
        private readonly Mock<IPreviewConfigurationsRepository> _previewConfigurationRepositoryMock;
        private readonly Mock<IGithubReposStore> _githubReposStoreMock;

        public PushWebhookServiceTests()
        {
            _githubBranchesRepositoryMock = new(MockBehavior.Strict);
            _previewConfigurationRepositoryMock = new(MockBehavior.Strict);
            _githubReposStoreMock = new(MockBehavior.Strict);

            _pushWebhookService = new PushWebhookService(
                _githubBranchesRepositoryMock.Object,
                _previewConfigurationRepositoryMock.Object,
                _githubReposStoreMock.Object
            );
        }

        #region HandleEventAsync
        [Fact]
        public async Task HandleEventAsync_NoSources()
        {
            var json = BuildValidJson();
            _githubReposStoreMock
                .Setup(c => c.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync((GithubRepo)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pushWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));
        }

        [Fact]
        public async Task HandleEventAsync_Created()
        {
            var json = BuildValidJson(created: true);
            _githubReposStoreMock
                .Setup(c => c.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new GithubRepo
                {
                    Id = 42
                });
            _githubBranchesRepositoryMock
                .Setup(g => g.CreateAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<GithubApiCommit>()))
                .ReturnsAsync((GithubBranch)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pushWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));
            _githubBranchesRepositoryMock.Verify(g => g.CreateAsync(42, "main", It.IsAny<GithubApiCommit>()));
        }

        [Fact]
        public async Task HandleEventAsync_Deleted_notfound()
        {
            var json = BuildValidJson(deleted: true);
            _githubReposStoreMock
                .Setup(c => c.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new GithubRepo
                {
                    Id = 42
                });
            _githubBranchesRepositoryMock
                .Setup(g => g.GetNonDeletedBranchByNameAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((GithubBranch)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pushWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));
            _githubBranchesRepositoryMock.Verify(g => g.GetNonDeletedBranchByNameAsync(42, "main"));
        }

        [Fact]
        public async Task HandleEventAsync_Deleted()
        {
            var json = BuildValidJson(deleted: true);
            var githubBranch = new GithubBranch();
            _githubReposStoreMock
                .Setup(c => c.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new GithubRepo
                {
                    Id= 42
                });
            _githubBranchesRepositoryMock
                .Setup(g => g.GetNonDeletedBranchByNameAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(githubBranch);
            _githubBranchesRepositoryMock
                .Setup(g => g.UpdateAsync(It.IsAny<GithubBranch>()))
                .Returns<GithubBranch>(gb => Task.FromResult(gb));
            _previewConfigurationRepositoryMock
                .Setup(p => p.DeleteByBranchAsync(It.IsAny<GithubBranch>()))
                .Returns(Task.CompletedTask);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pushWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));

            _githubBranchesRepositoryMock.Verify(g => g.GetNonDeletedBranchByNameAsync(42, "main"));
            _githubBranchesRepositoryMock.Verify(g => g.UpdateAsync(It.Is<GithubBranch>(b => b.IsDeleted == true)));
            _previewConfigurationRepositoryMock.Verify(p => p.DeleteByBranchAsync(githubBranch));
        }


        [Fact]
        public async Task HandleEventAsync_Any()
        {
            var json = BuildValidJson();
            var githubBranch = new GithubBranch();
            _githubReposStoreMock
                .Setup(c => c.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new GithubRepo
                {
                    Id = 42
                });
            _githubBranchesRepositoryMock
                .Setup(g => g.GetNonDeletedBranchByNameAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(githubBranch);
            _githubBranchesRepositoryMock
                .Setup(g => g.UpdateAsync(It.IsAny<GithubBranch>()))
                .Returns<GithubBranch>(gb => Task.FromResult(gb));
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pushWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));

            _githubBranchesRepositoryMock.Verify(g => g.GetNonDeletedBranchByNameAsync(42, "main"));
            _githubBranchesRepositoryMock.Verify(g => g.UpdateAsync(githubBranch));
        }


        private string BuildValidJson(bool created = false, bool deleted = false) => @"{
           ""created"": " + (created ? "true" : "false") + @",
           ""deleted"": " + (deleted ? "true" : "false") + @",
           ""ref"": ""refs/heads/main"",
           ""after"": ""52810785b09fa8f2c51418648dec2764873c424f"",
           ""head_commit"": {
                ""id"": ""6113728f27ae82c7b1a177c8d03f9e96e0adf246"",
                ""message"": ""Adding a .gitignore file""
           },
           ""repository"" : {
                ""id"": 1296269,
                ""html_url"": """ + _repositoryUrl + @"""
           },
           ""sender"": {
                ""login"": ""octocat"",
                ""id"": 1
           }
        }";
        #endregion
    }
}
