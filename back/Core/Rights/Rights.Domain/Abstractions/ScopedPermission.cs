using System.Collections.Generic;
using Tools;

namespace Rights.Domain.Abstractions
{
    public class ScopedPermission : ValueObject
    {
        public Operation Operation { get; set; }
        public AccessRightScope Scope { get; set; }
        public HashSet<int> EnvironmentPurposes { get; set; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return Operation;
                yield return Scope;
                foreach (var environmentPurpose in EnvironmentPurposes)
                {
                    yield return environmentPurpose;
                }
            }
        }
    }
}
