using FluentAssertions;
using Instances.Application.CodeSources;
using Instances.Application.Instances;
using Instances.Application.Webhooks.Github;
using Instances.Domain.CodeSources;
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
        private const string RepositoryUrl = "https://github.com/LuccaSA/repo";
        private const int PullRequestNumber = 42;

        private readonly PullRequestWebhookService _pullRequestWebhookService;

        private readonly Mock<ICodeSourcesRepository> _codeSourcesRepositoryMock;
        private readonly Mock<IGithubBranchesRepository> _githubBranchesRepositoryMock;
        private readonly Mock<IGithubPullRequestsRepository> _githubPullRequestRepositoryMock;
        private readonly Mock<IPreviewConfigurationsRepository> _previewConfigurationRepositoryMock;


        public PullRequestWebhookServiceTests()
        {
            _codeSourcesRepositoryMock = new Mock<ICodeSourcesRepository>(MockBehavior.Strict);
            _githubBranchesRepositoryMock = new Mock<IGithubBranchesRepository>(MockBehavior.Strict);
            _githubPullRequestRepositoryMock = new Mock<IGithubPullRequestsRepository>(MockBehavior.Strict);
            _previewConfigurationRepositoryMock = new Mock<IPreviewConfigurationsRepository>(MockBehavior.Strict);

            _pullRequestWebhookService = new PullRequestWebhookService(
                _codeSourcesRepositoryMock.Object,
                _githubBranchesRepositoryMock.Object,
                _githubPullRequestRepositoryMock.Object,
                _previewConfigurationRepositoryMock.Object
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
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource>());

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));
        }
        #endregion

        #region HandleOpenedActionAsync
        [Fact]
        public async Task HandleOpenedActionAsync_NotFound()
        {
            var json = BuildValidJson("opened");
            var codeSource = new CodeSource();
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource> { codeSource });
            _githubBranchesRepositoryMock
                .Setup(g => g.GetNonDeletedBranchByNameAsync(It.IsAny<CodeSource>(), It.IsAny<string>()))
                .ReturnsAsync((GithubBranch)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));
            _githubBranchesRepositoryMock.Verify(g => g.GetNonDeletedBranchByNameAsync(codeSource, "changes"));
        }

        [Fact]
        public async Task HandleOpenedActionAsync()
        {
            var json = BuildValidJson("opened");
            var codeSource = new CodeSource();
            var githubBranch = new GithubBranch();
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource> { codeSource });
            _githubBranchesRepositoryMock
                .Setup(g => g.GetNonDeletedBranchByNameAsync(It.IsAny<CodeSource>(), It.IsAny<string>()))
                .ReturnsAsync(githubBranch);
            _githubPullRequestRepositoryMock
                .Setup(p => p.CreateAsync(It.IsAny<GithubPullRequest>()))
                .Returns<GithubPullRequest>(p => Task.FromResult(p));
            _previewConfigurationRepositoryMock
                .Setup(p => p.CreateByPullRequestAsync(It.IsAny<GithubPullRequest>(), It.IsAny<GithubBranch>()))
                .Returns(Task.CompletedTask);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));
            _githubBranchesRepositoryMock.Verify(g => g.GetNonDeletedBranchByNameAsync(codeSource, "changes"));
            _githubPullRequestRepositoryMock.Verify(p => p.CreateAsync(It.IsAny<GithubPullRequest>()));
            _previewConfigurationRepositoryMock.Verify(p => p.CreateByPullRequestAsync(It.IsAny<GithubPullRequest>(), githubBranch));
        }
        #endregion

        #region HandleReopenedActionAsync
        [Fact]
        public async Task HandleRepoenedActionAsync_NotFound()
        {
            var json = BuildValidJson("reopened");
            var codeSource = new CodeSource();
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource> { codeSource });
            _githubBranchesRepositoryMock
                .Setup(g => g.GetNonDeletedBranchByNameAsync(It.IsAny<CodeSource>(), It.IsAny<string>()))
                .ReturnsAsync((GithubBranch)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));
            _githubBranchesRepositoryMock.Verify(g => g.GetNonDeletedBranchByNameAsync(codeSource, "changes"));
        }

        [Fact]
        public async Task HandleRepoenedActionAsync_PrNotFound()
        {
            var json = BuildValidJson("reopened");
            var codeSource = new CodeSource();
            var githubBranch = new GithubBranch();
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource> { codeSource });
            _githubBranchesRepositoryMock
                .Setup(g => g.GetNonDeletedBranchByNameAsync(It.IsAny<CodeSource>(), It.IsAny<string>()))
                .ReturnsAsync(githubBranch);
            _githubPullRequestRepositoryMock
                .Setup(p => p.GetByNumberAsync(It.IsAny<CodeSource>(), It.IsAny<int>()))
                .ReturnsAsync((GithubPullRequest)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));
            _githubBranchesRepositoryMock.Verify(g => g.GetNonDeletedBranchByNameAsync(codeSource, "changes"));
            _githubPullRequestRepositoryMock.Verify(p => p.GetByNumberAsync(codeSource, 42));
        }

        [Fact]
        public async Task HandleRepoenedActionAsync_Ok()
        {
            var json = BuildValidJson("reopened");
            var codeSource = new CodeSource();
            var githubBranch = new GithubBranch();
            var pullRequest = new GithubPullRequest();
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource> { codeSource });
            _githubBranchesRepositoryMock
                .Setup(g => g.GetNonDeletedBranchByNameAsync(It.IsAny<CodeSource>(), It.IsAny<string>()))
                .ReturnsAsync(githubBranch);
            _githubPullRequestRepositoryMock
                .Setup(p => p.GetByNumberAsync(It.IsAny<CodeSource>(), It.IsAny<int>()))
                .ReturnsAsync(pullRequest);
            _githubPullRequestRepositoryMock
                .Setup(p => p.UpdateAsync(It.IsAny<GithubPullRequest>()))
                .Returns<GithubPullRequest>(p => Task.FromResult(p));
            _previewConfigurationRepositoryMock
                .Setup(p => p.CreateByPullRequestAsync(It.IsAny<GithubPullRequest>(), It.IsAny<GithubBranch>()))
                .Returns(Task.CompletedTask);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));
            _githubBranchesRepositoryMock.Verify(g => g.GetNonDeletedBranchByNameAsync(codeSource, "changes"));
            _githubPullRequestRepositoryMock.Verify(p => p.GetByNumberAsync(codeSource, 42));
            _githubPullRequestRepositoryMock.Verify(p => p.UpdateAsync(pullRequest));
            _previewConfigurationRepositoryMock.Verify(p => p.CreateByPullRequestAsync(pullRequest, githubBranch));
        }
        #endregion

        #region HandleClosedActionAsync
        [Fact]
        public async Task HandleClosedActionAsync_PrNotFound()
        {
            var json = BuildValidJson("closed");
            var codeSource = new CodeSource();
            var githubBranch = new GithubBranch();
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource> { codeSource });
            _githubPullRequestRepositoryMock
                .Setup(p => p.GetByNumberAsync(It.IsAny<CodeSource>(), It.IsAny<int>()))
                .ReturnsAsync((GithubPullRequest)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));
            _githubPullRequestRepositoryMock.Verify(p => p.GetByNumberAsync(codeSource, 42));
        }

        [Fact]
        public async Task HandleClosedActionAsync_Merged()
        {
            var json = BuildValidJson("closed", merged: true);
            var codeSource = new CodeSource();
            var githubBranch = new GithubBranch();
            var pullRequest = new GithubPullRequest
            {
                IsOpened = true
            };
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource> { codeSource });
            _githubPullRequestRepositoryMock
                .Setup(p => p.GetByNumberAsync(It.IsAny<CodeSource>(), It.IsAny<int>()))
                .ReturnsAsync(pullRequest);
            _githubPullRequestRepositoryMock
                .Setup(p => p.UpdateAsync(It.IsAny<GithubPullRequest>()))
                .Returns<GithubPullRequest>(p => Task.FromResult(p));

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));
            _githubPullRequestRepositoryMock.Verify(p => p.GetByNumberAsync(codeSource, 42));
            _githubPullRequestRepositoryMock.Verify(p => p.UpdateAsync(pullRequest));

            pullRequest.IsOpened.Should().BeFalse();
            pullRequest.MergedAt.Should().BeCloseTo(new DateTime(2019, 05, 15, 15, 20, 33), TimeSpan.FromSeconds(1));
            pullRequest.ClosedAt.Should().BeNull();
        }

        [Fact]
        public async Task HandleClosedActionAsync_Closed()
        {
            var json = BuildValidJson("closed", closed: true);
            var codeSource = new CodeSource();
            var githubBranch = new GithubBranch();
            var pullRequest = new GithubPullRequest
            {
                IsOpened = true
            };
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource> { codeSource });
            _githubPullRequestRepositoryMock
                .Setup(p => p.GetByNumberAsync(It.IsAny<CodeSource>(), It.IsAny<int>()))
                .ReturnsAsync(pullRequest);
            _githubPullRequestRepositoryMock
                .Setup(p => p.UpdateAsync(It.IsAny<GithubPullRequest>()))
                .Returns<GithubPullRequest>(p => Task.FromResult(p));

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));
            _githubPullRequestRepositoryMock.Verify(p => p.GetByNumberAsync(codeSource, 42));
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
            var codeSource = new CodeSource();
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource> { codeSource });
            _githubBranchesRepositoryMock
                .Setup(p => p.GetNonDeletedBranchByNameAsync(It.IsAny<CodeSource>(), It.IsAny<string>()))
                .ReturnsAsync((GithubBranch)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));
            _githubBranchesRepositoryMock.Verify(p => p.GetNonDeletedBranchByNameAsync(codeSource, "changes"));
        }

        [Fact]
        public async Task HandleEditedActionAsync_PullRequestNotFound()
        {
            var json = BuildValidJson("edited");
            var codeSource = new CodeSource();
            var githubBranch = new GithubBranch();
            var pullRequest = new GithubPullRequest();
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource> { codeSource });
            _githubBranchesRepositoryMock
                .Setup(p => p.GetNonDeletedBranchByNameAsync(It.IsAny<CodeSource>(), It.IsAny<string>()))
                .ReturnsAsync(githubBranch);
            _githubPullRequestRepositoryMock
                .Setup(p => p.GetByNumberAsync(It.IsAny<CodeSource>(), It.IsAny<int>()))
                .ReturnsAsync((GithubPullRequest)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));
            _githubBranchesRepositoryMock.Verify(p => p.GetNonDeletedBranchByNameAsync(codeSource, "changes"));
            _githubPullRequestRepositoryMock.Verify(p => p.GetByNumberAsync(codeSource, 42));
        }

        [Fact]
        public async Task HandleEditedActionAsync_Ok()
        {
            var json = BuildValidJson("edited");
            var codeSource = new CodeSource();
            var githubBranch = new GithubBranch();
            var pullRequest = new GithubPullRequest();
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource> { codeSource });
            _githubBranchesRepositoryMock
                .Setup(p => p.GetNonDeletedBranchByNameAsync(It.IsAny<CodeSource>(), It.IsAny<string>()))
                .ReturnsAsync(githubBranch);
            _githubPullRequestRepositoryMock
                .Setup(p => p.GetByNumberAsync(It.IsAny<CodeSource>(), It.IsAny<int>()))
                .ReturnsAsync(pullRequest);
            _githubPullRequestRepositoryMock
                .Setup(p => p.UpdateAsync(It.IsAny<GithubPullRequest>()))
                .Returns<GithubPullRequest>(p => Task.FromResult(p));

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pullRequestWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));
            _githubBranchesRepositoryMock.Verify(p => p.GetNonDeletedBranchByNameAsync(codeSource, "changes"));
            _githubPullRequestRepositoryMock.Verify(p => p.GetByNumberAsync(codeSource, 42));
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
                ""html_url"": """ + RepositoryUrl + @"""
           },
           ""sender"": {
                ""login"": ""octocat"",
                ""id"": 1
           }
        }";
    }
}
