using System.Collections.Generic;
using System.Linq;

namespace Environments.Domain.Storage
{

    public abstract class PurposeAccessRight
    {
        public static PurposeAccessRight ForAll => new AllPurposeAccessRight();
        public static PurposeAccessRight ForSome(IEnumerable<int> purposes) => new SomePurposesAccessRight(purposes);

        protected PurposeAccessRight()
        { }
    }

    public class AllPurposeAccessRight : PurposeAccessRight
    {
        internal AllPurposeAccessRight()
        { }
    }

    public class SomePurposesAccessRight : PurposeAccessRight
    {
        public HashSet<EnvironmentPurpose> Purposes { get; }

        internal SomePurposesAccessRight(HashSet<EnvironmentPurpose> purposes)
        {
            Purposes = purposes;
        }

        internal SomePurposesAccessRight(IEnumerable<int> purposes)
            : this(purposes.Select(i => (EnvironmentPurpose)i).ToHashSet())
        { }
    }
}
