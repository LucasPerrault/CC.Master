using System.Net.Http;
using System.Text;
using Tools;

namespace Remote.Infra.Extensions
{
    public static class JsonExtensions
    {
        public static StringContent ToJsonPayload(this object content)
        {
            return new StringContent(Serializer.Serialize(content), Encoding.UTF8, "application/json");
        }
    }
}
