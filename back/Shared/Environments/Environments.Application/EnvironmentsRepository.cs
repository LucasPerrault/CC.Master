using Authentication.Domain;
using Environments.Domain;
using Environments.Infra.Storage.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Environment = Environments.Domain.Environment;

namespace Environments.Application
{
    public class EnvironmentsRepository
    {
        private readonly EnvironmentsStore _store;
        private readonly ClaimsPrincipal _principal;
        private readonly IEnvironmentFilter _filter;

        public EnvironmentsRepository(EnvironmentsStore store, ClaimsPrincipal principal, IEnvironmentFilter filter)
        {
            _store = store;
            _principal = principal;
            _filter = filter;
        }

        public List<Environment> Get()
        {
            return _principal switch
            {
                CloudControlUserClaimsPrincipal user => _store
                    .GetFilteredAsync(_filter.ReadAccessFilter(user.User))
                    .Take(50)
                    .ToList(),
                CloudControlApiKeyClaimsPrincipal apiKey => _store.GetAllAsync().Take(50).ToList(),
                _ => throw new ApplicationException("Unhandled ClaimsPrincipal type")
            };
        }
    }
}
