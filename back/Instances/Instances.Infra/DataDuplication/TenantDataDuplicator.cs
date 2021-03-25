using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Infra.DataDuplication
{
    public interface ITenantDataDuplicator
    {
        Task<DuplicateInstanceRequestDto> DuplicateOnRemoteAsync(TenantDataDuplication duplication);
    }

    public class DuplicateInstanceRequestDto
    {
        public int AuthorId { get; set; }
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

    public class TenantDataDuplicator : ITenantDataDuplicator
    {
        private readonly SqlScriptPicker _scriptPicker;

        public TenantDataDuplicator(SqlScriptPicker scriptPicker)
        {
            _scriptPicker = scriptPicker;
        }

        public async Task<DuplicateInstanceRequestDto> DuplicateOnRemoteAsync(TenantDataDuplication duplication)
        {

            var scripts = _scriptPicker.GetForDuplication(duplication);

            var sourceClusterUri = duplication.Source.ClusterName == duplication.Target.ClusterName
                ? null
                : GetUriFromClusterName(duplication.Source.ClusterName);

            return new DuplicateInstanceRequestDto
            {
                AuthorId = duplication.AuthorId,
                SourceTenant = new TenantDto
                {
                    Tenant = duplication.Source.Subdomain,
                    CcDataServerUri = sourceClusterUri
                },
                TargetTenant = duplication.Target.Subdomain,
                PostRestoreScripts = scripts.Select(uri => new UriLinkDto
                {
                    Uri = uri
                }).ToList()
            };
        }

        private Uri GetUriFromClusterName(string sourceCluster)
        {
            return new Uri($"cc-data.{sourceCluster}.lucca.local");
        }
    }
}
