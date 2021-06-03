using System.Text.Json;

namespace Remote.Infra.Extensions
{
    public static class StringExtensions
    {
        public static string AsSafeEndpoint(this string endpoint)
        {
            if (endpoint == null)
            {
                return null;
            }

            return endpoint.EndsWith('/') ? endpoint : $"{endpoint}/";
        }
    }
}
