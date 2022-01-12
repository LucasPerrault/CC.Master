using System.Collections.Generic;
using System.Linq;

namespace Tools
{
    public static class QueryParamsHelper
    {
        public static string ToQueryParams(Dictionary<string, string> dict)
        {
            return dict.Select(kvp => $"{kvp.Key}={kvp.Value}").Aggregate((s, s1) => $"{s}&{s1}");
        }
    }
}
