#nullable enable
using System.Net;

namespace IpFilter.Domain.Accessors
{
    public interface IIpAccessor
    {
        IPAddress? IpAddress { get; }
    }
}
