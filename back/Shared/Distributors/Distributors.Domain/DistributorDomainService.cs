using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Distributors.Domain;

public class Domain
{
    public string Value { get; }
    internal Domain(string value) => Value = value;
}

public interface IDistributorDomainService
{
    Domain GetDomain(string email);
    Task<bool> IsRegisteredAsync(Domain domain, int distributorId);
    Task<List<Domain>> GetAllRegistered(int distributorId);
}

public class DistributorDomainService : IDistributorDomainService
{
    private readonly IDistributorDomainsStore _store;

    public DistributorDomainService(IDistributorDomainsStore store)
    {
        _store = store;
    }

    public Domain GetDomain(string email)
    {
        return new Domain(email.Split('@').Last());
    }

    public async Task<bool> IsRegisteredAsync(Domain domain, int distributorId)
    {
        return ( await _store.GetByDistributorId(distributorId) ).Any(d => d.Domain == domain.Value);
    }

    public async Task<List<Domain>> GetAllRegistered(int distributorId)
    {
        return ( await _store.GetByDistributorId(distributorId) ).Select(v => new Domain(v.Domain)).ToList();
    }
}
