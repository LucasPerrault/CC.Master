using FluentAssertions;
using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github.Models;
using Instances.Infra.Storage;
using Instances.Infra.Storage.Stores;
using System;
using System.Collections.Generic;
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
            var codeSource1 =  await _dbContext.AddAsync(new CodeSource { Id = 1, Lifecycle = CodeSourceLifecycleStep.InProduction });
            var codeSource2 = await _dbContext.AddAsync(new CodeSource { Id = 2, Lifecycle = CodeSourceLifecycleStep.InProduction });
            var repo1 = new GithubRepo
            {
                Id = 10,
                CodeSources = new List<CodeSource> { codeSource1.Entity, codeSource2.Entity }
            };
            var repo2 = new GithubRepo
            {
                Id = 11,
                CodeSources = new List<CodeSource>()
            };
            await _dbContext.AddRangeAsync(new[] { repo1, repo2 });
            await _dbContext.SaveChangesAsync();

            // CREATE
            await _githubPullRequestsStore.CreateAsync(new GithubPullRequest
            {
                IsOpened = true,
                Number = 42,
                RepoId = repo1.Id
            });

            // GET
            var first = await _githubPullRequestsStore.GetFirstAsync(new GithubPullRequestFilter
            {
                Number = 42,
                RepoId = repo1.Id
            });
            var second = await _githubPullRequestsStore.GetFirstAsync(new GithubPullRequestFilter
            {
                Number = 42,
                RepoId = repo2.Id
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
                Number = 42,
                RepoId = repo1.Id
            });

            third.Should().NotBeNull();
            third.IsOpened.Should().BeFalse();
            third.MergedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(500));
        }
    }
}
