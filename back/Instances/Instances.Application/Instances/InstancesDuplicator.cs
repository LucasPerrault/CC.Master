using Instances.Domain.Instances;
using Instances.Domain.Shared;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public class InstancesDuplicator
    {
        private readonly ICcDataService _ccDataService;
        private readonly ISqlScriptPicker _scriptPicker;

        public InstancesDuplicator(ISqlScriptPicker scriptPicker, ICcDataService ccDataService)
        {
            _scriptPicker = scriptPicker;
            _ccDataService = ccDataService;
        }

        internal Task RequestRemoteDuplicationAsync
            (InstanceDuplication duplication, string callbackPath)
        {
            var scripts = _scriptPicker.GetForDuplication(duplication);

            var sourceClusterUri = duplication.SourceCluster == duplication.TargetCluster
                ? null
                : _ccDataService.GetCcDataBaseUri(duplication.SourceCluster);

            var duplicateInstanceRequest = new DuplicateInstanceRequestDto
            {
                SourceTenant = new TenantDto
                {
                    Tenant = duplication.SourceSubdomain,
                    CcDataServerUri = sourceClusterUri
                },
                TargetTenant = duplication.TargetSubdomain,
                PostRestoreScripts = scripts.Select(uri => new UriLinkDto { Uri = uri }).ToList()
            };

            return _ccDataService.StartDuplicateInstanceAsync
            (
                duplicateInstanceRequest,
                duplication.TargetCluster,
                callbackPath
            );
        }
    }
}
