using FluentAssertions;
using Instances.Application.CodeSources;
using Instances.Application.Instances;
using Instances.Application.Webhooks.Github;
using Instances.Domain.CodeSources;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Application.Tests.Webhooks.Github
{
    public class PullRequestWebhookServiceTests
    {
        private readonly Uri _repositoryUrl = new("https://github.com/LuccaSA/repo");
        private const int PullRequestNumber = 42;

        private readonly PullRequestWebhookService _pullRequestWebhookService;

        private readonly Mock<IGithubBranchesRepository> _githubBranchesRepositoryMock;
        private readonly Mock<IGithubPullRequestsRepository> _githubPullRequestRepositoryMock;
        private readonly Mock<IPreviewConfigurationsRepository> _previewConfigurationRepositoryMock;
        private readonly Mock<IGithubReposStore> _githubReposStoreMock;


        public PullRequestWebhookServiceTests()
        {
            _githubBranchesRepositoryMock = new(MockBehavior.Strict);
            _githubPullRequestRepositoryMock = new(MockBehavior.Strict);
            _previewConfigurationRepositoryMock = new(MockBehavior.Strict);
            _githubReposStoreMock = new(MockBehavior.Strict); 

            _pullRequestWebhookService = new PullRequestWebhookService(
                _githubBranchesRepositoryMock.Object,
                _githubPullRequestRepositoryMock.Object,
                _previewConfigurationRepositoryMock.Object,
                _githubReposStoreMock.Object
            );
        }

        #region HandleEventAsync
        [Fact]
        public async Task HandleEventAsync_NotSupported()
        {
            var json = BuildValidJson("test");

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);
        }

        [Fact]
        public async Task HandleEventAsync_Empty()
        {
            var json = BuildValidJson("opened");
            _githubReposStoreMock
                .Setup(c => c.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync((GithubRepo)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));
        }
        #endregion

        #region HandleOpenedActionAsync
        [Fact]
        public async Task HandleOpenedActionAsync_NotFound()
        {
            var json = BuildValidJson("opened");
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
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));
            _githubBranchesRepositoryMock.Verify(g => g.GetNonDeletedBranchByNameAsync(42, "changes"));
        }

        [Fact]
        public async Task HandleOpenedActionAsync()
        {
            var json = BuildValidJson("opened");
            var githubBranch = new GithubBranch();
            _githubReposStoreMock
                .Setup(c => c.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new GithubRepo { Id = 42 });
            _githubBranchesRepositoryMock
                .Setup(g => g.GetNonDeletedBranchByNameAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(githubBranch);
            _githubPullRequestRepositoryMock
                .Setup(p => p.CreateAsync(It.IsAny<GithubPullRequest>()))
                .Returns<GithubPullRequest>(p => Task.FromResult(p));
            _previewConfigurationRepositoryMock
                .Setup(p => p.CreateByPullRequestAsync(It.IsAny<GithubPullRequest>(), It.IsAny<GithubBranch>()))
                .Returns(Task.CompletedTask);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));
            _githubBranchesRepositoryMock.Verify(g => g.GetNonDeletedBranchByNameAsync(42, "changes"));
            _githubPullRequestRepositoryMock.Verify(p => p.CreateAsync(It.IsAny<GithubPullRequest>()));
            _previewConfigurationRepositoryMock.Verify(p => p.CreateByPullRequestAsync(It.IsAny<GithubPullRequest>(), githubBranch));
        }
        #endregion

        #region HandleReopenedActionAsync
        [Fact]
        public async Task HandleRepoenedActionAsync_NotFound()
        {
            var json = BuildValidJson("reopened");
            _githubReposStoreMock
                .Setup(c => c.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new GithubRepo { Id = 42 });
            _githubBranchesRepositoryMock
                .Setup(g => g.GetNonDeletedBranchByNameAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((GithubBranch)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));
            _githubBranchesRepositoryMock.Verify(g => g.GetNonDeletedBranchByNameAsync(42, "changes"));
        }

        [Fact]
        public async Task HandleRepoenedActionAsync_PrNotFound()
        {
            var json = BuildValidJson("reopened");
            var githubBranch = new GithubBranch();
            _githubReposStoreMock
                .Setup(c => c.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new GithubRepo { Id = 44 });
            _githubBranchesRepositoryMock
                .Setup(g => g.GetNonDeletedBranchByNameAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(githubBranch);
            _githubPullRequestRepositoryMock
                .Setup(p => p.GetByNumberAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((GithubPullRequest)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));
            _githubBranchesRepositoryMock.Verify(g => g.GetNonDeletedBranchByNameAsync(44, "changes"));
            _githubPullRequestRepositoryMock.Verify(p => p.GetByNumberAsync(44, 42));
        }

        [Fact]
        public async Task HandleRepoenedActionAsync_Ok()
        {
            var json = BuildValidJson("reopened");
            var githubBranch = new GithubBranch();
            var pullRequest = new GithubPullRequest();
            _githubReposStoreMock
                .Setup(c => c.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new GithubRepo { Id = 21 });
            _githubBranchesRepositoryMock
                .Setup(g => g.GetNonDeletedBranchByNameAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(githubBranch);
            _githubPullRequestRepositoryMock
                .Setup(p => p.GetByNumberAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(pullRequest);
            _githubPullRequestRepositoryMock
                .Setup(p => p.UpdateAsync(It.IsAny<GithubPullRequest>()))
                .Returns<GithubPullRequest>(p => Task.FromResult(p));
            _previewConfigurationRepositoryMock
                .Setup(p => p.CreateByPullRequestAsync(It.IsAny<GithubPullRequest>(), It.IsAny<GithubBranch>()))
                .Returns(Task.CompletedTask);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));
            _githubBranchesRepositoryMock.Verify(g => g.GetNonDeletedBranchByNameAsync(21, "changes"));
            _githubPullRequestRepositoryMock.Verify(p => p.GetByNumberAsync(21, 42));
            _githubPullRequestRepositoryMock.Verify(p => p.UpdateAsync(pullRequest));
            _previewConfigurationRepositoryMock.Verify(p => p.CreateByPullRequestAsync(pullRequest, githubBranch));
        }
        #endregion

        #region HandleClosedActionAsync
        [Fact]
        public async Task HandleClosedActionAsync_PrNotFound()
        {
            var json = BuildValidJson("closed");
            var githubBranch = new GithubBranch();
            _githubReposStoreMock
                .Setup(c => c.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new GithubRepo { Id = 60 });
            _githubPullRequestRepositoryMock
                .Setup(p => p.GetByNumberAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((GithubPullRequest)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));
            _githubPullRequestRepositoryMock.Verify(p => p.GetByNumberAsync(60, 42));
        }

        [Fact]
        public async Task HandleClosedActionAsync_Merged()
        {
            var json = BuildValidJson("closed", merged: true);
            var githubBranch = new GithubBranch();
            var pullRequest = new GithubPullRequest
            {
                IsOpened = true
            };
            _githubReposStoreMock
                .Setup(c => c.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new GithubRepo { Id = 80 });
            _githubPullRequestRepositoryMock
                .Setup(p => p.GetByNumberAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(pullRequest);
            _githubPullRequestRepositoryMock
                .Setup(p => p.UpdateAsync(It.IsAny<GithubPullRequest>()))
                .Returns<GithubPullRequest>(p => Task.FromResult(p));

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));
            _githubPullRequestRepositoryMock.Verify(p => p.GetByNumberAsync(80, 42));
            _githubPullRequestRepositoryMock.Verify(p => p.UpdateAsync(pullRequest));

            pullRequest.IsOpened.Should().BeFalse();
            pullRequest.MergedAt.Should().BeCloseTo(new DateTime(2019, 05, 15, 15, 20, 33), TimeSpan.FromSeconds(1));
            pullRequest.ClosedAt.Should().BeNull();
        }

        [Fact]
        public async Task HandleClosedActionAsync_Closed()
        {
            var json = BuildValidJson("closed", closed: true);
            var githubBranch = new GithubBranch();
            var pullRequest = new GithubPullRequest
            {
                IsOpened = true
            };
            _githubReposStoreMock
                .Setup(c => c.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new GithubRepo { Id = 90 });
            _githubPullRequestRepositoryMock
                .Setup(p => p.GetByNumberAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(pullRequest);
            _githubPullRequestRepositoryMock
                .Setup(p => p.UpdateAsync(It.IsAny<GithubPullRequest>()))
                .Returns<GithubPullRequest>(p => Task.FromResult(p));

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));
            _githubPullRequestRepositoryMock.Verify(p => p.GetByNumberAsync(90, 42));
            _githubPullRequestRepositoryMock.Verify(p => p.UpdateAsync(pullRequest));

            pullRequest.IsOpened.Should().BeFalse();
            pullRequest.ClosedAt.Should().BeCloseTo(new DateTime(2019, 05, 15, 15, 20, 33), TimeSpan.FromSeconds(1));
            pullRequest.MergedAt.Should().BeNull();
        }
        #endregion

        #region HandleEditedActionAsync
        [Fact]
        public async Task HandleEditedActionAsync_BranchNotFound()
        {
            var json = BuildValidJson("edited");
            _githubReposStoreMock
                .Setup(c => c.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new GithubRepo { Id = 42 });
            _githubBranchesRepositoryMock
                .Setup(p => p.GetNonDeletedBranchByNameAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((GithubBranch)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));
            _githubBranchesRepositoryMock.Verify(p => p.GetNonDeletedBranchByNameAsync(42, "changes"));
        }

        [Fact]
        public async Task HandleEditedActionAsync_PullRequestNotFound()
        {
            var json = BuildValidJson("edited");
            var githubBranch = new GithubBranch();
            var pullRequest = new GithubPullRequest();
            _githubReposStoreMock
                .Setup(c => c.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new GithubRepo { Id = 24 });
            _githubBranchesRepositoryMock
                .Setup(p => p.GetNonDeletedBranchByNameAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(githubBranch);
            _githubPullRequestRepositoryMock
                .Setup(p => p.GetByNumberAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((GithubPullRequest)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));
            _githubBranchesRepositoryMock.Verify(p => p.GetNonDeletedBranchByNameAsync(24, "changes"));
            _githubPullRequestRepositoryMock.Verify(p => p.GetByNumberAsync(24, 42));
        }

        [Fact]
        public async Task HandleEditedActionAsync_Ok()
        {
            var json = BuildValidJson("edited");
            var githubBranch = new GithubBranch();
            var pullRequest = new GithubPullRequest();
            _githubReposStoreMock
                .Setup(c => c.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new GithubRepo { Id = 24 });
            _githubBranchesRepositoryMock
                .Setup(p => p.GetNonDeletedBranchByNameAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(githubBranch);
            _githubPullRequestRepositoryMock
                .Setup(p => p.GetByNumberAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(pullRequest);
            _githubPullRequestRepositoryMock
                .Setup(p => p.UpdateAsync(It.IsAny<GithubPullRequest>()))
                .Returns<GithubPullRequest>(p => Task.FromResult(p));

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(_repositoryUrl));
            _githubBranchesRepositoryMock.Verify(p => p.GetNonDeletedBranchByNameAsync(24, "changes"));
            _githubPullRequestRepositoryMock.Verify(p => p.GetByNumberAsync(24, 42));
            _githubPullRequestRepositoryMock.Verify(p => p.UpdateAsync(pullRequest));

            pullRequest.Title.Should().Be("Adding a .gitignore file");
        }
        #endregion

        private string BuildValidJson(string action, bool merged = false, bool closed = false) => @"{
           ""action"": """ + action + @""",
           ""number"": " + PullRequestNumber + @",
           ""pull_request"": {
                " + (merged ? @"""merged_at"": ""2019-05-15T15:20:33Z""," : "") + @"
                " + (closed ? @"""closed_at"": ""2019-05-15T15:20:33Z""," : "") + @"
                ""title"": ""Adding a .gitignore file"",
                ""head"": {
                    ""ref"" : ""changes""
                }
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
    }
}
