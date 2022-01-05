using Distributors.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distributors.Domain;

public interface IDistributorDomainsStore
{
    Task<List<DistributorDomain>> GetByDistributorId(int distributorId);
}
