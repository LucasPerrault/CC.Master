using Billing.Contracts.Domain.Common;
using Billing.Products.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tools;

namespace Billing.Contracts.Infra.Services
{
    public class RemoteCountDetail
    {
        public int Value { get; set; }
        public int LegalEntityId { get; set; }
    }

    public class CountApiClient
    {
        private readonly HttpClient _httpClient;

        public CountApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<RemoteCountDetail>> GetRemoteCountDetailsAsync
        (
            string environmentUri,
            CountRemoteService.RemoteCountTarget target,
            AccountingPeriod period,
            List<Product> products
        )
        {
            var url = GetUrl(environmentUri, target, period, products);
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            var apiContainer = await Serializer.DeserializeAsync<CountRemoteApiContainer>(stream);
            return apiContainer.Data.Items;
        }

        private string GetUrl(string environmentUri, CountRemoteService.RemoteCountTarget target, AccountingPeriod period, List<Product> products)
        {
            var remoteProduct = GetRemoteProduct(target.ProductId, products);
            var applicationId = remoteProduct.ApplicationId;
            var moduleName = remoteProduct.Module;
            var billingMode = ((int)target.BillingMode).ToString();
            var month = $"{period: yyyy-MM}";

            var queryParams = new Dictionary<string, string>
            {
                ["legalEntityIds"] = string.Join(',', target.EstablishmentIds),
                ["applicationId"] = applicationId,
                ["billingMode"] = billingMode,
                ["moduleName"] = moduleName,
                ["month"] = month,
            };

            return $"{environmentUri}/api/v3/counts?{QueryParamsHelper.ToQueryParams(queryParams)}";
        }

        private RemoteProduct GetRemoteProduct(int productId, List<Product> products)
        {
            const string defaultModule = "main";
            var product = products.Single(p => p.Id == productId);
            var parent = product.ParentId.HasValue
                ? products.Single(p => p.Id == product.ParentId)
                : null;

            return new RemoteProduct
            {
                ApplicationId = parent?.ApplicationCode ?? product.ApplicationCode,
                Module = parent is not null ? product.ApplicationCode : defaultModule,
            };
        }

        private class RemoteProduct
        {
            public string ApplicationId { get; set; }
            public string Module { get; set; }
        }
        public class CountRemoteApiContainer
        {
            public CountRemoteApiDtoList Data { get; set; }
        }

        public class CountRemoteApiDtoList
        {
            public List<RemoteCountDetail> Items { get; set; }
        }
    }
}
