using Authentication.Domain;
using System;
using System.Collections.Concurrent;
using Users.Domain;

namespace Authentication.Infra.Services
{
    public class AuthenticationCache
    {
        private static readonly TimeSpan CacheInvalidation = TimeSpan.FromSeconds(10);

        private readonly ConcurrentDictionary<Guid, CachedUser> _cachedUsers;
        private readonly ConcurrentDictionary<Guid, CachedApiKey> _cachedApiKeys;

        public AuthenticationCache()
        {
            _cachedUsers = new ConcurrentDictionary<Guid, CachedUser>();
            _cachedApiKeys = new ConcurrentDictionary<Guid, CachedApiKey>();
        }

        internal bool TryGetUser(Guid token, out User user)
        {
            user = null;
            return false;
            // if (!_cachedUsers.TryGetValue(token, out CachedUser cachedUser))
            // {
            //     user = null;
            //     return false;
            // }
            //
            // user = GetValidPrincipal(cachedUser);
            // return user != null;
        }

        internal bool TryGetApiKey(Guid token, out ApiKey apiKey)
        {
            apiKey = null;
            return false;
            // if (!_cachedApiKeys.TryGetValue(token, out CachedApiKey cachedApiKey))
            // {
            //     apiKey = null;
            //     return false;
            // }
            //
            // apiKey = GetValidPrincipal(cachedApiKey);
            // return apiKey != null;
        }

        internal void Cache(Guid token, User user)
        {
            _cachedUsers[token] = new CachedUser(user);
        }

        internal void Cache(Guid token, ApiKey apiKey)
        {
            _cachedApiKeys[token] = new CachedApiKey(apiKey);
        }

        private T GetValidPrincipal<T>(CachedPrincipal<T> cachedPrincipal) where T : class
        {
            return cachedPrincipal.InvalidationDate < DateTime.Now
                ? null
                : cachedPrincipal.Principal;
        }

        private class CachedPrincipal<T> where T : class
        {
            public T Principal { get; }
            public DateTime InvalidationDate { get; }

            public CachedPrincipal(T principal)
            {
                Principal = principal;
                InvalidationDate = DateTime.Now.Add(CacheInvalidation);
            }
        }

        private class CachedUser : CachedPrincipal<User>
        {
            public CachedUser(User principal) : base(principal)
            { }
        }

        private class CachedApiKey : CachedPrincipal<ApiKey>
        {
            public CachedApiKey(ApiKey principal) : base(principal)
            { }
        }
    }
}
