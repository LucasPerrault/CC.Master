using System.Text.Json;

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

        public static string Serialize(object o)
        {
            return JsonSerializer.Serialize(o);
        }
    }
}
