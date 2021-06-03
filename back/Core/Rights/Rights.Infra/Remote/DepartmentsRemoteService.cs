using Partenaires.Infra.Services;
using Rights.Infra.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rights.Infra.Remote
{
    public class DepartmentsRemoteService : PartenairesService
    {
        public DepartmentsRemoteService(HttpClient httpClient) : base(httpClient)
        { }

        protected override string RemoteApiDescription => "Partenaires departments";

        internal async Task<IReadOnlyCollection<Department>> GetDepartmentsAsync()
        {
            var queryParams = new Dictionary<string, string>();

            var departmentsResponse = await GetObjectCollectionResponseAsync<Department>(queryParams);

            return departmentsResponse.Data.Items.ToList();
        }
    }
}
