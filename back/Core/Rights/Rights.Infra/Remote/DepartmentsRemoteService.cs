using Remote.Infra.Services;
using Rights.Infra.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rights.Infra.Remote
{
    public class DepartmentsRemoteService
    {
        private readonly RestApiV3HttpClientHelper _httpClientHelper;
        public DepartmentsRemoteService(HttpClient httpClient)
        {
            _httpClientHelper = new RestApiV3HttpClientHelper(httpClient,"Partenaires departments");
        }

        internal async Task<IReadOnlyCollection<Department>> GetDepartmentsAsync()
        {

            var departmentsResponse = await _httpClientHelper.GetObjectCollectionResponseAsync<Department>();

            return departmentsResponse.Data.Items.ToList();
        }
    }
}
