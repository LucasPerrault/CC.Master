using FluentAssertions;
using Instances.Domain.CodeSources;
using Instances.Infra.CodeSources;
using Xunit;

namespace Instances.Infra.Tests.CodeSources
{
    public class JenkinsCodeSourceBuildUrlServiceTests
    {
        private readonly JenkinsCodeSourceBuildUrlService _jenkinsCodeSourceBuildUrlService;

        public JenkinsCodeSourceBuildUrlServiceTests()
        {
            _jenkinsCodeSourceBuildUrlService = new JenkinsCodeSourceBuildUrlService();
        }

        #region IsValidBuildNumber
        [Theory]
        [InlineData("lastSuccessfulBuild", true)]
        [InlineData("lastBuild", true)]
        [InlineData("next-build", false)]
        [InlineData("12743", true)]
        [InlineData("3", true)]
        [InlineData("3.4", false)]
        [InlineData("", true)]
        public void IsValidBuildNumber(string input, bool isValid)
        {
            _jenkinsCodeSourceBuildUrlService.IsValidBuildNumber(input).Should().Be(isValid);
        }
        #endregion

        #region GenerateBuildUrl
        [Theory]
        [InlineData("http://jenkins.lucca.test", null, "http://jenkins.lucca.test/job/my-branch/lastSuccessfulBuild")]
        [InlineData("http://jenkins.lucca.test/", null, "http://jenkins.lucca.test/job/my-branch/lastSuccessfulBuild")]
        [InlineData("http://jenkins.lucca.test", "lastSuccessfulBuild", "http://jenkins.lucca.test/job/my-branch/lastSuccessfulBuild")]
        [InlineData("http://jenkins.lucca.test/", "1234", "http://jenkins.lucca.test/job/my-branch/1234")]
        public void GenerateBuildUrl(string jenkinsProjectUrl, string buildNumber, string expected)
        {
            var branchName = "my-branch";
            var codeSource = new CodeSource
            {
                JenkinsProjectUrl = jenkinsProjectUrl
            };

            var generatedUrl = _jenkinsCodeSourceBuildUrlService.GenerateBuildUrl(codeSource, branchName, buildNumber);

            generatedUrl.Should().Be(expected);
        }
        #endregion
    }
}
