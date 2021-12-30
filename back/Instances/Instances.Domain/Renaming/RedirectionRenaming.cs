using Environments.Domain.ExtensionInterface;
using Instances.Domain.Instances;
using System;
using System.Threading.Tasks;
using Tools;

namespace Instances.Domain.Renaming
{
    public class RedirectionRenaming : IEnvironmentRenamingExtension
    {
        private readonly IRedirectionIisAdministration _redirectionIisAdministration;
        private readonly IDnsService _dnsService;

        public RedirectionRenaming(IRedirectionIisAdministration redirectionIisAdminstration, IDnsService dnsService)
        {
            _redirectionIisAdministration = redirectionIisAdminstration;
            _dnsService = dnsService;
        }

        public string ExtensionName => "Old domain redirection";
        public bool ShouldExecute(IEnvironmentRenamingExtensionParameters parameters) => parameters.HasRedirection;

        public async Task RenameAsync(Environments.Domain.Environment environment, string newName)
        {
            var domain = environment.Domain.GetDescription();
            await _redirectionIisAdministration.CreateRedirectionAsync(
                environment.Subdomain,
                newName,
                domain,
                DateOnly.FromDateTime(DateTime.Now).AddMonths(3)
            );
            await _redirectionIisAdministration.BindDomainAsync(newName, domain);
            await _dnsService.CreateAsync(DnsEntry.ForRedirection(environment.Subdomain));
        }
    }
}
