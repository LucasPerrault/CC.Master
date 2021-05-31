using Rights.Domain.Filtering;
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace Environments.Domain.Storage
{
    public class EnvironmentAccessRight : ValueObject
    {
        public AccessRight AccessRight { get; }
        public PurposeAccessRight Purposes { get; }

        public EnvironmentAccessRight(AccessRight accessRight, PurposeAccessRight purposes)
        {
            AccessRight = accessRight;
            Purposes = purposes;
        }

        public static List<EnvironmentAccessRight> Everything => new List<EnvironmentAccessRight>
        {
            new EnvironmentAccessRight(AccessRight.All, PurposeAccessRight.ForAll)
        };

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return Purposes;
                yield return AccessRight;
            }
        }
    }

    public abstract class PurposeAccessRight : ValueObject
    {
        public static PurposeAccessRight ForAll => new AllPurposeAccessRight();
        public static PurposeAccessRight ForSome(IEnumerable<int> purposes) => new SomePurposesAccessRight(purposes);
        public static PurposeAccessRight ForSome(params EnvironmentPurpose[] purposes) => new SomePurposesAccessRight(purposes.ToHashSet());
    }

    public class AllPurposeAccessRight : PurposeAccessRight
    {
        internal AllPurposeAccessRight()
        { }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return typeof(AllPurposeAccessRight);
            }
        }
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

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return typeof(EnvironmentPurpose);
                foreach (var purpose in Purposes)
                {
                    yield return purpose;
                }
            }
        }
    }
}
