using Instances.Domain.Demos;
using System.Threading.Tasks;

namespace Instances.Infra.Demos
{
    public class DemoDuplicator
    {

        public DemoDuplicator()
        { }

        public async Task DuplicateAsync(DemoDuplication duplication)
        {
            // check current user rights

            // ensure subdomain is available, source demo exists, distributor selection is okay...

            // determine sql scripts to run on newly copied db

            // Request remote database creation

            // create demo on local

            // change password for all users

            // create SSO for demo if necessary
        }
    }
}
