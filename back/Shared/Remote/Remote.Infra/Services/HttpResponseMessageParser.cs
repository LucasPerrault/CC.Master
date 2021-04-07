using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Remote.Infra.Services
{
    public class HttpResponseMessageParser
    {
        private readonly JsonSerializer _jsonSerializer;

        public HttpResponseMessageParser(JsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public async Task<T> ParseAsync<T>(HttpResponseMessage message)
        {
            await using var responseStream = await message.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(responseStream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            return _jsonSerializer.Deserialize<T>(jsonTextReader);
        }
    }
}
