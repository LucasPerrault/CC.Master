using Instances.Domain.CodeSources;
using System;
using System.Collections.Generic;

namespace Instances.Domain.Github.Models;

public class GithubRepo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Uri Url { get; set; }
    public List<CodeSource> CodeSources { get; set; }
    public List<GithubBranch> GithubBranches { get; set; }
}
