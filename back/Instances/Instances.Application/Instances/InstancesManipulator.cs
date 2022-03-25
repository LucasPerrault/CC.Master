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

        internal async Task RequestRemoteDuplicationAsync
            (InstanceDuplication duplication, InstanceDuplicationOptions duplicationOptions)
        {
            var postBufferServerRestoreScripts = (await _scriptPicker.GetForCleaningAsync(duplication))
                .Union(await _scriptPicker.GetExtraForDuplicationAsync(duplication));

            if (duplicationOptions.WithAnonymization)
            {
                postBufferServerRestoreScripts = postBufferServerRestoreScripts.Union(await _scriptPicker.GetForAnonymizationAsync(duplication));
            }

            var preRestoreScripts = await _scriptPicker.GetPreRestoreScriptsAsync(duplication, duplicationOptions);
            var postRestoreScripts = await _scriptPicker.GetPostRestoreScriptsAsync(duplication, duplicationOptions);

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
                SkipBufferServer = duplicationOptions.SkipBufferServer,
                TargetTenant = duplication.TargetSubdomain,
                PostBufferServerRestoreScripts = postBufferServerRestoreScripts.Select(uri => new UriLinkDto { Uri = uri }).ToList(),
                PreRestoreScripts = preRestoreScripts.Select(uri => new UriLinkDto { Uri = uri }).ToList(),
                PostRestoreScripts = postRestoreScripts.Select(uri => new UriLinkDto { Uri = uri }).ToList(),
            };

            await _ccDataService.StartDuplicateInstanceAsync
            (
                duplicateInstanceRequest,
                duplication.TargetCluster,
                duplicationOptions.CallbackPath
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
