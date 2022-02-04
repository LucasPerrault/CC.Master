using Instances.Domain.Github.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Github;

public interface IGithubReposStore
{
    Task<GithubRepo> CreateAsync(Uri url);
    Task<GithubRepo> GetByIdAsync(int repoId);
    Task<GithubRepo> GetByUriAsync(Uri repoUri);
    Task<List<GithubRepo>> GetAllAsync();
}
