using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tools
{
    public static class Serializer
    {

        public static T Deserialize<T>(string content)
        {
            return JsonSerializer.Deserialize<T>
            (
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        }

        public static async Task<T> DeserializeAsync<T>(Stream content)
        {
            return await JsonSerializer.DeserializeAsync<T>
            (
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        }

        public static string Serialize(object o)
        {
            return JsonSerializer.Serialize(o, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}
