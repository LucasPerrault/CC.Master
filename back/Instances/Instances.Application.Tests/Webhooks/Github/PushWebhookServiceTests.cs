using FluentAssertions;
using Instances.Application.CodeSources;
using Instances.Application.Instances;
using Instances.Application.Webhooks.Github;
using Instances.Domain.CodeSources;
using Instances.Domain.Github.Models;
using Lucca.Core.Shared.Domain.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Application.Tests.Webhooks.Github
{
    public class PushWebhookServiceTests
    {
        private const string RepositoryUrl = "https://github.com/LuccaSA/repo";
        private readonly PushWebhookService _pushWebhookService;
        private readonly Mock<ICodeSourcesRepository> _codeSourcesRepositoryMock;
        private readonly Mock<IGithubBranchesRepository> _githubBranchesRepositoryMock;
        private readonly Mock<IPreviewConfigurationsRepository> _previewConfigurationRepositoryMock;

        public PushWebhookServiceTests()
        {
            _codeSourcesRepositoryMock = new Mock<ICodeSourcesRepository>(MockBehavior.Strict);
            _githubBranchesRepositoryMock = new Mock<IGithubBranchesRepository>(MockBehavior.Strict);
            _previewConfigurationRepositoryMock = new Mock<IPreviewConfigurationsRepository>(MockBehavior.Strict);

            _pushWebhookService = new PushWebhookService(
                _codeSourcesRepositoryMock.Object,
                _githubBranchesRepositoryMock.Object,
                _previewConfigurationRepositoryMock.Object
            );
        }

        #region HandleEventAsync
        [Fact]
        public async Task HandleEventAsync_NoSources()
        {
            var json = BuildValidJson();
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource>());

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pushWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));
        }

        [Fact]
        public async Task HandleEventAsync_Created()
        {
            var json = BuildValidJson(created: true);
            var codeSource = new CodeSource
            {
                GithubRepo = RepositoryUrl
            };
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource> { codeSource });
            _githubBranchesRepositoryMock
                .Setup(g => g.CreateAsync(It.IsAny<List<CodeSource>>(), It.IsAny<string>(), It.IsAny<GithubApiCommit>()))
                .ReturnsAsync((GithubBranch)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pushWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));

            _githubBranchesRepositoryMock.Verify(g => g.CreateAsync(It.IsAny<List<CodeSource>>(), "main", It.IsAny<GithubApiCommit>()));
        }

        [Fact]
        public async Task HandleEventAsync_Deleted_notfound()
        {
            var json = BuildValidJson(deleted: true);
            var codeSource = new CodeSource
            {
                GithubRepo = RepositoryUrl
            };
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource> { codeSource });
            _githubBranchesRepositoryMock
                .Setup(g => g.GetNonDeletedBranchByNameAsync(It.IsAny<CodeSource>(), It.IsAny<string>()))
                .ReturnsAsync((GithubBranch)null);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pushWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));
            _githubBranchesRepositoryMock.Verify(g => g.GetNonDeletedBranchByNameAsync(codeSource, "main"));
        }

        [Fact]
        public async Task HandleEventAsync_Deleted()
        {
            var json = BuildValidJson(deleted: true);
            var githubBranch = new GithubBranch();
            var codeSource = new CodeSource
            {
                GithubRepo = RepositoryUrl
            };
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource> { codeSource });
            _githubBranchesRepositoryMock
                .Setup(g => g.GetNonDeletedBranchByNameAsync(It.IsAny<CodeSource>(), It.IsAny<string>()))
                .ReturnsAsync(githubBranch);
            _githubBranchesRepositoryMock
                .Setup(g => g.UpdateAsync(It.IsAny<GithubBranch>()))
                .Returns<GithubBranch>(gb => Task.FromResult(gb));
            _previewConfigurationRepositoryMock
                .Setup(p => p.DeleteByBranchAsync(It.IsAny<GithubBranch>()))
                .Returns(Task.CompletedTask);

            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pushWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));

            _githubBranchesRepositoryMock.Verify(g => g.GetNonDeletedBranchByNameAsync(codeSource, "main"));
            _githubBranchesRepositoryMock.Verify(g => g.UpdateAsync(It.Is<GithubBranch>(b => b.IsDeleted == true)));
            _previewConfigurationRepositoryMock.Verify(p => p.DeleteByBranchAsync(githubBranch));
        }


        [Fact]
        public async Task HandleEventAsync_Any()
        {
            var json = BuildValidJson();
            var githubBranch = new GithubBranch();
            var codeSource = new CodeSource
            {
                GithubRepo = RepositoryUrl
            };
            _codeSourcesRepositoryMock
                .Setup(c => c.GetNonDeletedByRepositoryUrlAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<CodeSource> { codeSource });
            _githubBranchesRepositoryMock
                .Setup(g => g.GetNonDeletedBranchByNameAsync(It.IsAny<CodeSource>(), It.IsAny<string>()))
                .ReturnsAsync(githubBranch);
            _githubBranchesRepositoryMock
                .Setup(g => g.UpdateAsync(It.IsAny<GithubBranch>()))
                .Returns<GithubBranch>(gb => Task.FromResult(gb));
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await _pushWebhookService.HandleEventAsync(stream);

            _codeSourcesRepositoryMock.Verify(c => c.GetNonDeletedByRepositoryUrlAsync(RepositoryUrl));

            _githubBranchesRepositoryMock.Verify(g => g.GetNonDeletedBranchByNameAsync(codeSource, "main"));
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
                ""html_url"": """ + RepositoryUrl + @"""
           },
           ""sender"": {
                ""login"": ""octocat"",
                ""id"": 1
           }
        }";
        #endregion
    }
}
