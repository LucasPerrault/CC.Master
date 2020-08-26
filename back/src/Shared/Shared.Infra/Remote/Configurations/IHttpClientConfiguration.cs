using System;

namespace Shared.Infra.Remote.Configurations
{
    public interface IHttpClientConfiguration
    { }

    public class HostHttpClientConfiguration : IHttpClientConfiguration
    {
        public Uri Endpoint { get; set; }
    }
}
