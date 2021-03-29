using Instances.Domain.Instances;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Instances.Application.Instances
{
    public class InstancesDuplicator
    {
        private readonly ISqlScriptPicker _scriptPicker;

        public InstancesDuplicator(ISqlScriptPicker scriptPicker)
        {
            _scriptPicker = scriptPicker;
        }

        internal void RequestRemoteDuplicationAsync(InstanceDuplication duplication)
        {
            var scripts = _scriptPicker.GetForDuplication(duplication);

            var sourceClusterUri = duplication.SourceCluster == duplication.TargetCluster
                ? null
                : GetUriFromClusterName(duplication.SourceCluster);

            var dto = new DuplicateInstanceRequestDto
            {
                SourceTenant = new TenantDto
                {
                    Tenant = duplication.SourceSubdomain,
                    CcDataServerUri = sourceClusterUri
                },
                TargetTenant = duplication.TargetSubdomain,
                PostRestoreScripts = scripts.Select(uri => new UriLinkDto { Uri = uri }).ToList()
            };

            // call CC.Data
        }

        private Uri GetUriFromClusterName(string sourceCluster)
        {
            return new Uri($"http://cc-data.{sourceCluster}.lucca.local");
        }

    }

    public class DuplicateInstanceRequestDto
    {
        public TenantDto SourceTenant { get; set; }
        public string TargetTenant { get; set; }
        public List<UriLinkDto> PostRestoreScripts { get; set; }
    }

    public class UriLinkDto
    {
        public Uri Uri { get; set; }
        public string AuthorizationHeader { get; set; }
    }

    public class TenantDto
    {
        public string Tenant { get; set; }
        public Uri CcDataServerUri { get; set; }
    }
}
