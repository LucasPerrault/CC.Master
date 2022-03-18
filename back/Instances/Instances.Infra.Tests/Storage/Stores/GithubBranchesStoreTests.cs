using FluentAssertions;
using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github.Models;
using Instances.Infra.Storage;
using Instances.Infra.Storage.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testing.Infra;
using Xunit;

namespace Instances.Infra.Tests.Storage.Stores
{
    public class GithubBranchesStoreTests
    {
        private readonly InstancesDbContext _dbContext;
        private readonly GithubBranchesStore _githubBranchesStore;

        public GithubBranchesStoreTests()
        {
            _dbContext = InMemoryDbHelper.InitialiseDb<InstancesDbContext>("Instances", o => new InstancesDbContext(o));

            _githubBranchesStore = new GithubBranchesStore(_dbContext);
        }

        [Fact]
        public async Task Create_Update_Get_Async()
        {
            var codeSource1 = await _dbContext.AddAsync(new CodeSource { Id = 1, Lifecycle = CodeSourceLifecycleStep.InProduction });
            var codeSource2 = await _dbContext.AddAsync(new CodeSource { Id = 2, Lifecycle = CodeSourceLifecycleStep.InProduction });
            await _dbContext.AddAsync(new CodeSource { Id = 3, Lifecycle = CodeSourceLifecycleStep.InProduction });

            var githubRepo = new GithubRepo
            {
                Id = 10,
                CodeSources = new List<CodeSource>
                {
                    codeSource1.Entity,
                    codeSource2.Entity
                }
            };
            await _dbContext.AddAsync(githubRepo);
            await _dbContext.SaveChangesAsync();

            var githubBranch = new GithubBranch
            {
                Id = 42,
                Name = "myBranch",
                Repo = githubRepo,
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };

            // CREATE
            await _githubBranchesStore.CreateAsync(githubBranch);

            // GET
            var first = await _githubBranchesStore.GetFirstAsync(new GithubBranchFilter
            {
                RepoIds = new HashSet<int> { githubRepo.Id },
                Name = "myBranch"
            });
            var second = await _githubBranchesStore.GetFirstAsync(new GithubBranchFilter
            {
                RepoIds = new HashSet<int> { githubRepo.Id },
                Name = "myBranch"
            });
            var third = await _githubBranchesStore.GetFirstAsync(new GithubBranchFilter
            {
                RepoIds = new HashSet<int> { githubRepo.Id },
                Name = "badBranch"
            });

            first.Should().NotBeNull();
            second.Should().NotBeNull();
            third.Should().BeNull();
            first.Id.Should().Be(42);
            second.Id.Should().Be(42);


            // UPDATE
            second.HeadCommitMessage = "myMessage";
            await _githubBranchesStore.UpdateAsync(second);

            // GET
            var fourth = await _githubBranchesStore.GetFirstAsync(new GithubBranchFilter
            {
                RepoIds = new HashSet<int> { githubRepo.Id },
                Name = "myBranch"
            });

            fourth.Should().NotBeNull();
            fourth.HeadCommitMessage.Should().Be(second.HeadCommitMessage);
        }

        [Fact]
        public async Task GetProductionBranchesAsync()
        {
            var codeSource1 = new CodeSource { Id = 1, Lifecycle = CodeSourceLifecycleStep.InProduction };
            var codeSource2 = new CodeSource { Id = 2, Lifecycle = CodeSourceLifecycleStep.InProduction };
            var repo1 = new GithubRepo
            {
                Id = 10,
                CodeSources = new() { codeSource1 }
            };
            var repo2 = new GithubRepo
            {
                Id = 11,
                CodeSources = new() { codeSource2 }
            };
            await _dbContext.AddRangeAsync(new[]
            {
                codeSource1,
                codeSource2
            });
            await _dbContext.AddRangeAsync(new[]
            {
                repo1,
                repo2
            });
            await _dbContext.AddRangeAsync(new[]
            {
                new CodeSourceProductionVersion { Id = 1, BranchName = "v0.1.0", CodeSourceId = codeSource1.Id },
                new CodeSourceProductionVersion { Id = 2, BranchName = "v0.2.0", CodeSourceId = codeSource1.Id },
                new CodeSourceProductionVersion { Id = 3, BranchName = "v1.0.0", CodeSourceId = codeSource2.Id },
                new CodeSourceProductionVersion { Id = 4, BranchName = "v0.3.0", CodeSourceId = codeSource1.Id }
            });
            await _dbContext.AddRangeAsync(new[]
            {
                new GithubBranch { Id= 1, Name = "branch1", HelmChart = "helm1", RepoId = repo1.Id },
                new GithubBranch { Id= 2, Name = "v0.1.0", HelmChart = "helm2", RepoId = repo1.Id },
                new GithubBranch { Id= 3, Name = "v0.2.0", HelmChart = "helm3", RepoId = repo1.Id },
                new GithubBranch { Id= 4, Name = "v1.0.0", HelmChart = "helm4", RepoId = repo1.Id },
                new GithubBranch { Id= 5, Name = "v0.3.0", HelmChart = "helm5", RepoId = repo1.Id },
            });

            await _dbContext.SaveChangesAsync();

            var result = await _githubBranchesStore.GetProductionBranchesAsync(new GithubBranchFilter
            {
                RepoIds = new HashSet<int> { repo1.Id },
            });

            result.Should().HaveCount(1);
            result.First().HelmChart.Should().Be("helm5");
        }
    }
}
