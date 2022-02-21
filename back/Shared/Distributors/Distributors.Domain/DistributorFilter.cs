using System.Collections.Generic;

namespace Distributors.Domain;

public class DistributorFilter
{
    public HashSet<string> Search { get; set; } = new HashSet<string>();
}
