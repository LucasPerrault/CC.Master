using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using Tools;

namespace Remote.Infra.Extensions
{
    public static class JsonExtensions
    {
        public static StringContent ToJsonPayload(this object content, params JsonConverter[] converters)
        {
            return new StringContent(Serializer.Serialize(content, converters), Encoding.UTF8, "application/json");
        }
    }
}
