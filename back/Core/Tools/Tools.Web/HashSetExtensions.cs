using System.Collections.Generic;
using System.Linq;

namespace Tools.Web
{
    public static class HashSetExtensions
    {
        public static CompareBoolean ToCompareBoolean(this HashSet<bool> bools)
        {
            if (bools.Count == 1)
            {
                return bools.Single()
                    ? CompareBoolean.TrueOnly
                    : CompareBoolean.FalseOnly;
            }

            return CompareBoolean.Bypass;
        }
    }
}
