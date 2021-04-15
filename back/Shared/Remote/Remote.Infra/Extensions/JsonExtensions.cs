using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;

namespace Remote.Infra.Extensions
{
    public static class JsonExtensions
    {
        public static StringContent ToJsonPayload(this object content)
        {
            var jObject = JObject.FromObject(content);
            return new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
        }
    }
}
