using Instances.Domain.CodeSources;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Instances.Domain.Github.Models;

public class GithubRepo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Uri Url { get; set; }
    [JsonIgnore]
    public List<CodeSource> CodeSources { get; set; }
    [JsonIgnore]
    public List<GithubBranch> GithubBranches { get; set; }
}
