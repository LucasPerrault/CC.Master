using FluentAssertions;
using Instances.Infra.Iis;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Infra.Tests.Iis
{
    public class RedirectionIisAdministrationTests : IDisposable
    {
        private readonly RedirectionIisAdministration _redirectionIisAdministration;
        private readonly RedirectionIisConfiguration _redirectionIisConfiguration;

        private readonly string _tempDirectory;

        public RedirectionIisAdministrationTests()
        {
            _tempDirectory = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDirectory);
            _redirectionIisConfiguration = new RedirectionIisConfiguration
            {
                ApplicationHost = Path.Join(_tempDirectory, "applicationHost.conf"),
                RedirectionConf = Path.Join(_tempDirectory, "test.json")
            };
            _redirectionIisAdministration = new RedirectionIisAdministration(_redirectionIisConfiguration);
        }

        #region BindDomainAsync
        [Fact]
        public async Task BindDomainAsync()
        {
            await TestDiffFileAsync(
                "ApplicationHost.conf",
                _redirectionIisConfiguration.ApplicationHost,
                () => _redirectionIisAdministration.BindDomainAsync("test", "ilucca.net")
            );
        }
        #endregion

        #region CreateRedirectionAsync
        [Fact]
        public async Task CreateRedirectionAsync()
        {
            await TestDiffFileAsync(
                "Resources.json",
                _redirectionIisConfiguration.RedirectionConf,
                () => _redirectionIisAdministration.CreateRedirectionAsync("old-tenant", "new-tenant", "ilucca.net", new DateOnly(2021, 12, 24))
            );
        }
        #endregion

        private async Task TestDiffFileAsync(string resourceFileName, string tempFile, Func<Task> testAsync)
        {
            using (var input = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Instances.Infra.Tests.Iis.Resources.input{resourceFileName}"))
            using (var output = File.Create(tempFile))
            {
                await input.CopyToAsync(output);
            }

            await testAsync();

            var result = await File.ReadAllTextAsync(tempFile);
            using var expectedStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Instances.Infra.Tests.Iis.Resources.expected{resourceFileName}");
            var expected = await new StreamReader(expectedStream).ReadToEndAsync();

            result.Should().Be(expected);
        }

        public void Dispose()
        {
            Directory.Delete(_tempDirectory, true);
        }
    }
}
