using Environments.Domain.ExtensionInterface;
using Instances.Domain.Instances;
using System.Threading.Tasks;

namespace Instances.Domain.Renaming
{
    public class InstanceDnsRenaming : IEnvironmentRenamingExtension
    {
        private readonly IDnsService _dnsService;

        public InstanceDnsRenaming(IDnsService dnsService)
        {
            _dnsService = dnsService;
        }

        public string ExtensionName => "Dns renaming";

        public async Task RenameAsync(Environments.Domain.Environment environment, string newName)
        {
            await _dnsService.CreateAsync(DnsEntry.ForProduction(newName, environment.Cluster, environment.Domain));
            await _dnsService.CreateAsync(DnsEntry.ForPreview(newName, environment.Cluster));
            await _dnsService.CreateAsync(DnsEntry.ForPreview($"static-{newName}", environment.Cluster));
        }
    }
}
