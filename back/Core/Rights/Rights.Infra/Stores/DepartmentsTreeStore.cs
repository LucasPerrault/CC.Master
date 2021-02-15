using Lucca.Core.Rights.Abstractions;
using Lucca.Core.Rights.Abstractions.Stores;
using Lucca.Core.Shared.Domain.Utils;
using Rights.Infra.Models;
using Rights.Infra.Remote;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rights.Infra.Stores
{
    public class DepartmentsTreeStore : IDepartmentsTreeStore
    {
        private readonly DepartmentsRemoteService _departmentsService;

        public DepartmentsTreeStore(DepartmentsRemoteService departmentsService)
        {
            _departmentsService = departmentsService;
        }

        public async Task<ITree<IDepartment>> GetTreeAsync() => BuildTree(await GetApiDepartmentsAsync());

        private Task<IReadOnlyCollection<Department>> GetApiDepartmentsAsync()
        {
            return _departmentsService.GetDepartmentsAsync();
        }

        private ITree<IDepartment> BuildTree(IEnumerable<Department> depts) => Tree.Factory(depts);
    }
}
