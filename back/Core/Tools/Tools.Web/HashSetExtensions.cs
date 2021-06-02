using System.Collections.Generic;
using System.Linq;

namespace Tools.Web
{
    public static class HashSetExtensions
    {
        public static BoolCombination ToBoolCombination(this HashSet<bool> bools)
        {
            if (bools.Count == 1)
            {
                return bools.Single()
                    ? BoolCombination.TrueOnly
                    : BoolCombination.FalseOnly;
            }

            return BoolCombination.Both;
        }
    }
}
