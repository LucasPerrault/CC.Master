using Instances.Domain.CodeSources;
using System;

namespace Instances.Application.Instances.Dtos
{
    public class CreateCodeSourceDto
    {
        public Uri RepoUrl { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string JenkinsProjectName { get; set; }
        public string JenkinsProjectUrl { get; set; }
        public CodeSourceType Type { get; set; }
        public CodeSourceLifecycleStep Lifecycle { get; set; }
        public CodeSourceConfig Config { get; set; }
    }
}
