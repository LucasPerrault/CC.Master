using Instances.Domain.Instances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public interface IInstancesDuplicator
    {
        Task<DuplicateInstanceRequestDto> GetDuplicationInstanceRequestAsync(Guid duplicationId);
    }

    public class InstancesDuplicator : IInstancesDuplicator
    {
        private readonly IInstanceDuplicationsStore _duplicationsStore;
        private readonly ISqlScriptPicker _scriptPicker;

        public InstancesDuplicator(IInstanceDuplicationsStore duplicationsStore, ISqlScriptPicker scriptPicker)
        {
            _duplicationsStore = duplicationsStore;
            _scriptPicker = scriptPicker;
        }

        public async Task<DuplicateInstanceRequestDto> GetDuplicationInstanceRequestAsync(Guid duplicationId)
        {
            var duplication = await _duplicationsStore.GetAsync(duplicationId);

            var scripts = _scriptPicker.GetForDuplication(duplication);

            var sourceClusterUri = duplication.SourceCluster == duplication.TargetCluster
                ? null
                : GetUriFromClusterName(duplication.SourceCluster);

            return new DuplicateInstanceRequestDto
            {
                AuthorId = duplication.AuthorId,
                SourceTenant = new TenantDto
                {
                    Tenant = duplication.SourceSubdomain,
                    CcDataServerUri = sourceClusterUri
                },
                TargetTenant = duplication.TargetSubdomain,
                PostRestoreScripts = scripts.Select(uri => new UriLinkDto { Uri = uri }).ToList()
            };
        }

        private Uri GetUriFromClusterName(string sourceCluster)
        {
            return new Uri($"cc-data.{sourceCluster}.lucca.local");
        }

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
}
