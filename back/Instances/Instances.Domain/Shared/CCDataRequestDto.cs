using System;
using System.Collections.Generic;

namespace Instances.Domain.Shared
{
    public class DuplicateInstanceRequestDto
    {
        public TenantDto SourceTenant { get; init; }
        public string TargetTenant { get; init; }
        public bool SkipBufferServer { get; init; }
        public List<UriLinkDto> PostBufferServerRestoreScripts { get; init; }
        public List<UriLinkDto> PreRestoreScripts { get; init; }
        public List<UriLinkDto> PostRestoreScripts { get; init; }
    }

    public class CreateInstanceBackupRequestDto
    {
        public CreateInstanceBackupRequestDto(string tenant)
        {
            Tenant = new TenantDto
            {
                Tenant = tenant,
                CcDataServerUri = null,
            };
        }

        public TenantDto Tenant { get; private set; }
    }

    public class ResetInstanceCacheRequestDto
    {
        public string TenantHost { get; set; }
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
