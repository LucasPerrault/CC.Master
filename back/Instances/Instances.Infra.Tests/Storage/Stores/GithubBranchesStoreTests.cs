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
            var codeSource1 = new CodeSource { Id = 1 };
            var codeSource2 = new CodeSource { Id = 2 };
            await _dbContext.AddAsync(new CodeSource { Id = codeSource1.Id, Lifecycle = CodeSourceLifecycleStep.InProduction });
            await _dbContext.AddAsync(new CodeSource { Id = codeSource2.Id, Lifecycle = CodeSourceLifecycleStep.InProduction });
            await _dbContext.AddAsync(new CodeSource { Id = 3, Lifecycle = CodeSourceLifecycleStep.InProduction });
            await _dbContext.SaveChangesAsync();

            codeSource1 = await _dbContext.Set<CodeSource>().FindAsync(codeSource1.Id);
            codeSource2 = await _dbContext.Set<CodeSource>().FindAsync(codeSource2.Id);

            var githubBranch = new GithubBranch
            {
                Id = 42,
                Name = "myBranch",
                CodeSources = new List<CodeSource>
                {
                    codeSource1,
                    codeSource2
                },
                IsDeleted = false,
                CreatedAt = DateTime.Now
            };

            // CREATE
            await _githubBranchesStore.CreateAsync(githubBranch);

            // GET
            var first = await _githubBranchesStore.GetFirstAsync(new GithubBranchFilter
            {
                CodeSourceId = codeSource1.Id,
                Name = "myBranch"
            });
            var second = await _githubBranchesStore.GetFirstAsync(new GithubBranchFilter
            {
                CodeSourceId = codeSource2.Id,
                Name = "myBranch"
            });
            var third = await _githubBranchesStore.GetFirstAsync(new GithubBranchFilter
            {
                CodeSourceId = codeSource2.Id,
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
                CodeSourceId = codeSource2.Id,
                Name = "myBranch"
            });

            fourth.Should().NotBeNull();
            fourth.HeadCommitMessage.Should().Be(second.HeadCommitMessage);
        }
    }
}
