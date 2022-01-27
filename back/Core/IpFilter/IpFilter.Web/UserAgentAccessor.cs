using IpFilter.Domain.Accessors;
using Microsoft.AspNetCore.Http;

namespace IpFilter.Web;

public class UserAgentAccessor : IUserAgentAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAgentAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserAgent => _httpContextAccessor.HttpContext?.Request?.Headers?.UserAgent ?? "";
}
