using System;
using System.Collections.Generic;

namespace Instances.Domain.Shared
{
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
