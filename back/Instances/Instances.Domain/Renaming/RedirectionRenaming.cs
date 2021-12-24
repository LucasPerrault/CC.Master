using Environments.Domain.ExtensionInterface;
using System;
using System.Threading.Tasks;
using Tools;

namespace Instances.Domain.Renaming
{
    public class RedirectionRenaming : IEnvironmentRenamingExtension
    {
        private readonly IRedirectionIisAdministration _redirectionIisAdministration;

        public RedirectionRenaming(IRedirectionIisAdministration redirectionIisAdminstration)
        {
            _redirectionIisAdministration = redirectionIisAdminstration;
        }

        public string ExtensionName => "Old domain redirection";

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
        }
    }
}
