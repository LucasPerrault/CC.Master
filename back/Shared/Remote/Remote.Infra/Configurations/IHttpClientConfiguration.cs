using System;

namespace Remote.Infra.Configurations
{
    public interface IHttpClientConfiguration
    { }

    public class HostHttpClientConfiguration : IHttpClientConfiguration
    {
        public Uri Endpoint { get; set; }
    }
}
