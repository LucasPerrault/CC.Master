using Environments.Domain;
using Instances.Domain.Instances;
using Instances.Domain.Renaming;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Environment = Environments.Domain.Environment;

namespace Instances.Domain.Tests.Renaming
{
    public class RedirectionRenamingTests
    {
        private readonly Mock<IRedirectionIisAdministration> _redirectionIisAdministrationMock;
        private readonly Mock<IDnsService> _dnsServiceMock;
        private readonly RedirectionRenaming _redirectionRenaming;

        public RedirectionRenamingTests()
        {
            _redirectionIisAdministrationMock = new Mock<IRedirectionIisAdministration>(MockBehavior.Strict);
            _dnsServiceMock = new Mock<IDnsService>(MockBehavior.Strict);
            _redirectionRenaming = new RedirectionRenaming(_redirectionIisAdministrationMock.Object, _dnsServiceMock.Object);
        }

        #region RenameAsync
        [Fact]
        public async Task RenameAsync()
        {
            var oldName = "old-instance";
            var newName = "new-instance";

            _redirectionIisAdministrationMock
                .Setup(r => r.CreateRedirectionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateOnly>()))
                .Returns(Task.CompletedTask);
            _redirectionIisAdministrationMock
                .Setup(r => r.BindDomainAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            _dnsServiceMock
                .Setup(d => d.CreateAsync(It.IsAny<DnsEntry>()))
                .Returns(Task.CompletedTask);

            await _redirectionRenaming.RenameAsync(new Environment
            {
                Domain = EnvironmentDomain.ILuccaDotNet,
                Subdomain = oldName
            }, newName);

            _redirectionIisAdministrationMock
                .Verify(r => r.CreateRedirectionAsync(
                    oldName,
                    newName,
                    "ilucca.net",
                    It.Is<DateOnly>(d => d.Month == DateTime.Now.AddMonths(3).Month)));
            _redirectionIisAdministrationMock.Verify(r => r.BindDomainAsync(oldName, "ilucca.net"));
            _dnsServiceMock.Verify(d => d.CreateAsync(It.Is<DnsEntry>(d => d.Subdomain == oldName && d.Cluster == IDnsService.RedirectionCluster)));
        }

        #endregion
    }
}
