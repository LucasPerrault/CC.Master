using Lucca.Core.Rights.Abstractions.Stores;
using Lucca.Core.Shared.Domain.Utils;
using System.Threading.Tasks;

namespace Rights.Infra.Stores
{
    public class ActorsStore : IActorsStore
    {
        public Task<IReadableGraph<int>> GetActorsGraphAsync() => Task.FromResult(BuildEmptyGraph());

        private IReadableGraph<int> BuildEmptyGraph()
        {
            return new Graph<int>();
        }
    }
}
