using Lucca.Core.Rights.Abstractions;
using System.Collections.Generic;

namespace Rights.Domain.Abstractions
{
    public class ScopedPermission
    {
        public Operation Operation { get; set; }
        public Scope Scope { get; set; }
        public HashSet<int> EnvironmentPurposes { get; set; }
    }
}
