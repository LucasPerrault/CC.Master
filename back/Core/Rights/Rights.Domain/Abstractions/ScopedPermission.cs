using System.Collections.Generic;

namespace Rights.Domain.Abstractions
{
    public class ScopedPermission
    {
        public Operation Operation { get; set; }
        public AccessRightScope Scope { get; set; }
        public HashSet<int> EnvironmentPurposes { get; set; }
    }
}
