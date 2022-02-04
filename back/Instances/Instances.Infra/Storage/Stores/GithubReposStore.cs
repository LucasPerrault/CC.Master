using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Infra.Storage.Stores;

public class GithubReposStore : IGithubReposStore
{
    private readonly InstancesDbContext _dbContext;

    public GithubReposStore(InstancesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GithubRepo> CreateAsync(Uri url)
    {
        var repo = _dbContext.Add(new GithubRepo
        {
            Name = url.PathAndQuery.Split("/").Last(),
            Url = url
        });
        await _dbContext.SaveChangesAsync();
        return repo.Entity;
    }

    public Task<List<GithubRepo>> GetAllAsync()
    {
        return _dbContext
            .Set<GithubRepo>()
            .ToListAsync();
    }

    public Task<GithubRepo> GetByIdAsync(int repoId)
    {
        return _dbContext
            .Set<GithubRepo>()
            .FirstOrDefaultAsync(gr => gr.Id == repoId);
    }

    public Task<GithubRepo> GetByUriAsync(Uri repoUri)
    {
        return _dbContext
            .Set<GithubRepo>()
            .FirstOrDefaultAsync(gr => gr.Url == repoUri);
    }
}
