using System;

namespace Instances.Application.Instances
{
    public abstract class HelmRequest
    {

        public bool ShouldBeStable { get;}
        protected internal HelmRequest(bool shouldBeStable)
        {
            ShouldBeStable = shouldBeStable;
        }

        public static HelmRequest ForRepo(string repoName, string gitRef, bool shouldBeStable) => new SpecificRepoHelmRequest(repoName, gitRef, shouldBeStable);
        public static HelmRequest StableForRepo(string repoName, string gitRef, bool shouldBeStable) => new SpecificRepoHelmRequest(repoName, gitRef, shouldBeStable);
        public static HelmRequest ForAllRepos() => new AllStableHelmRequest();
    }

    public class AllStableHelmRequest : HelmRequest
    {
        internal AllStableHelmRequest() : base(shouldBeStable: true)
        { }
    }

    public class SpecificRepoHelmRequest : HelmRequest
    {
        public string RepoName { get; }
        public string GitRef { get; }

        internal SpecificRepoHelmRequest(string repoName, string gitRef, bool shouldBeStable) : base(shouldBeStable)
        {
            if (string.IsNullOrEmpty(repoName))
            {
                throw new ApplicationException("Attempted to create specific repo without a repo name");
            }

            RepoName = repoName;
            GitRef = gitRef;
        }
    }
}
