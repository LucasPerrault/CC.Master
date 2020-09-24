using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Authentication.Infra.Services
{
    public class SessionKeyService
    {
        private const string _sessionKeyKey = "sessionKey";

        public SessionKeyService()
        { }

        public bool ContainsSessionKey(IQueryCollection queryCollection)
        {
            return queryCollection.ContainsKey(_sessionKeyKey);
        }

        public string GetSessionKey(IQueryCollection queryCollection)
        {
            return queryCollection[_sessionKeyKey].SingleOrDefault();
        }
    }
}
