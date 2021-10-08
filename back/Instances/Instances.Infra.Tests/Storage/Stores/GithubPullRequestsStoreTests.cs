using FluentAssertions;
using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github.Models;
using Instances.Infra.Storage;
using Instances.Infra.Storage.Models;
using Instances.Infra.Storage.Stores;
using System;
using System.Threading.Tasks;
using Testing.Infra;
using Xunit;

namespace Instances.Infra.Tests.Storage.Stores
{
    public class GithubPullRequestsStoreTests
    {
        private readonly InstancesDbContext _dbContext;
        private readonly GithubPullRequestsStore _githubPullRequestsStore;

        public GithubPullRequestsStoreTests()
        {
            _dbContext = InMemoryDbHelper.InitialiseDb<InstancesDbContext>("Instances", o => new InstancesDbContext(o));

            _githubPullRequestsStore = new GithubPullRequestsStore(_dbContext);
        }

        [Fact]
        public async Task Create_Update_Get_Async()
        {
            var codeSource = new CodeSource { Id = 2 };
            await _dbContext.AddAsync(new StoredCodeSource { Id = 1, Lifecycle = CodeSourceLifecycleStep.InProduction });
            await _dbContext.AddAsync(new StoredCodeSource { Id = codeSource.Id, Lifecycle = CodeSourceLifecycleStep.InProduction });

            // CREATE
            await _githubPullRequestsStore.CreateAsync(new GithubPullRequest
            {
                IsOpened = true,
                Number = 42,
                CodeSources = new System.Collections.Generic.List<CodeSource> { codeSource }
            });

            // GET
            var first = await _githubPullRequestsStore.GetFirstAsync(new GithubPullRequestFilter
            {
                CodeSourceId = codeSource.Id,
                Number = 42
            });
            var second = await _githubPullRequestsStore.GetFirstAsync(new GithubPullRequestFilter
            {
                CodeSourceId = 42,
                Number = 42
            });

            first.Should().NotBeNull();
            first.IsOpened.Should().BeTrue();
            first.MergedAt.Should().BeNull();
            second.Should().BeNull();


            // UPDATE
            first.IsOpened = false;
            first.MergedAt = DateTime.Now;
            await _githubPullRequestsStore.UpdateAsync(first);

            // GET
            var third = await _githubPullRequestsStore.GetFirstAsync(new GithubPullRequestFilter
            {
                CodeSourceId = codeSource.Id,
                Number = 42
            });

            third.Should().NotBeNull();
            third.IsOpened.Should().BeFalse();
            third.MergedAt.Should().BeCloseTo(DateTime.Now);
        }
    }
}
