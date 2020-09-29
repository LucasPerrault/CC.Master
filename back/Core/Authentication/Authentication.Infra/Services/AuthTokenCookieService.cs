using Microsoft.AspNetCore.Http;

namespace Authentication.Infra.Services
{
    public class AuthTokenCookieService
    {

        private const string _authTokenCookieKey = "authToken";

        public AuthTokenCookieService()
        { }

        public void SetAuthTokenCookie(HttpContext context, string token)
        {
            context.Response.Cookies.Append(_authTokenCookieKey, token);
        }

        public void InvalidateAuthTokenCookie(HttpContext context)
        {
            context.Response.Cookies.Delete(_authTokenCookieKey);
        }
    }
}
