using Instances.Domain.Instances;
using Instances.Domain.Renaming;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Domain.Tests.Renaming
{
    public class InstanceDnsRenamingTests
    {
        private readonly Mock<IDnsService> _dnsServiceMock;
        private readonly InstanceDnsRenaming _instanceDnsRenaming;

        public InstanceDnsRenamingTests()
        {
            _dnsServiceMock = new Mock<IDnsService>(MockBehavior.Strict);
            _instanceDnsRenaming = new InstanceDnsRenaming(_dnsServiceMock.Object);
        }

        #region RenameAsync
        [Fact]
        public async Task RenameAsync_Ok()
        {
            var cluster = "cluster1";
            var newName = "instance-name";

            _dnsServiceMock
                .Setup(dns => dns.CreateAsync(It.IsAny<DnsEntry>()))
                .Returns(Task.CompletedTask);

            await _instanceDnsRenaming.RenameAsync(new Environments.Domain.Environment
            {
                Cluster = cluster,
                Domain = Environments.Domain.EnvironmentDomain.ILuccaDotNet
            }, newName);

            _dnsServiceMock.Verify(dns => dns.CreateAsync(It.IsAny<DnsEntry>()), Times.Exactly(3));
            _dnsServiceMock.Verify(dns => dns.CreateAsync(It.Is<DnsEntry>(dns =>
                dns.Cluster == cluster && dns.Subdomain == newName && dns.Zone == DnsEntryZone.RbxProductions
            )));
            _dnsServiceMock.Verify(dns => dns.CreateAsync(It.Is<DnsEntry>(dns =>
                dns.Cluster == cluster && dns.Subdomain == newName && dns.Zone == DnsEntryZone.Previews
            )));
            _dnsServiceMock.Verify(dns => dns.CreateAsync(It.Is<DnsEntry>(dns =>
                dns.Cluster == cluster && dns.Subdomain == $"static-{newName}" && dns.Zone == DnsEntryZone.Previews
            )));
        }

        #endregion

    }
}
