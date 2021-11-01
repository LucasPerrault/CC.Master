using Environments.Domain;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public class InstancesManipulator
    {
        private readonly ICcDataService _ccDataService;
        private readonly ISqlScriptPicker _scriptPicker;

        public InstancesManipulator(ISqlScriptPicker scriptPicker, ICcDataService ccDataService)
        {
            _scriptPicker = scriptPicker;
            _ccDataService = ccDataService;
        }

        internal Task RequestRemoteDuplicationAsync
            (InstanceDuplication duplication, bool skipBufferServer, string callbackPath)
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
                SkipBufferServer = skipBufferServer,
                TargetTenant = duplication.TargetSubdomain,
                PostBufferServerRestoreScripts = new List<UriLinkDto>(),
                PreRestoreScripts = new List<UriLinkDto>(),
                PostRestoreScripts = scripts.Select(uri => new UriLinkDto { Uri = uri }).ToList()
            };

            return _ccDataService.StartDuplicateInstanceAsync
            (
                duplicateInstanceRequest,
                duplication.TargetCluster,
                callbackPath
            );
        }

        internal Task RequestRemoteBackupAsync(Environment environment, InstanceType instanceType)
        {
            return _ccDataService.CreateInstanceBackupAsync
            (
                new CreateInstanceBackupRequestDto(environment.Subdomain),
                environment.GetInstanceExecutingCluster(instanceType)
            );
        }

        internal Task RequestResetInstanceCacheAsync(Environment environment, InstanceType instanceType)
        {
            return _ccDataService.ResetInstanceCacheAsync
            (
                new ResetInstanceCacheRequestDto { TenantHost = environment.GetInstanceHost(instanceType) },
                environment.GetInstanceExecutingCluster(instanceType)
            );
        }

    }
}
