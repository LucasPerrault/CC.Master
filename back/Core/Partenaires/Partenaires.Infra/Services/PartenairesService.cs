using Newtonsoft.Json;
using Remote.Infra.Services;
using System.Net.Http;

namespace Partenaires.Infra.Services
{
    public abstract class PartenairesService : RestApiV3HostRemoteService
    {

        public PartenairesService(HttpClient httpClient, JsonSerializer jsonSerializer)
            : base(httpClient, jsonSerializer)
        { }
    }
}
