using System.Collections.Generic;
using System.Linq;

namespace Storage.Infra.Extensions
{
    public static class HashSetExtensions
    {
        public static HashSet<string> Sanitize(this HashSet<string> hashSet)
        {
            return ( hashSet ?? new HashSet<string>() )
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .ToHashSet();
        }
    }
}
