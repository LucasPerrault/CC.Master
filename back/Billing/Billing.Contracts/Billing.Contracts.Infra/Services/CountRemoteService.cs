using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Counts;
using Billing.Contracts.Domain.Offers;
using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using Email.Domain;
using Environments.Domain;
using Environments.Domain.Storage;
using Lucca.Emails.Client.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;

namespace Billing.Contracts.Infra.Services
{
    public class CountRemoteService : ICountRemoteService
    {
        private readonly CountApiClient _apiClient;
        private readonly IEmailService _emailService;
        private readonly IProductsStore _productsStore;
        private readonly IEnvironmentsStore _environmentsStore;
        private readonly EstablishmentCountService _establishmentCountService;

        public CountRemoteService
        (
            IEmailService emailService,
            IEnvironmentsStore environmentsStore,
            EstablishmentCountService establishmentCountService,
            CountApiClient apiClient,
            IProductsStore productsStore
        )
        {
            _emailService = emailService;
            _environmentsStore = environmentsStore;
            _establishmentCountService = establishmentCountService;
            _apiClient = apiClient;
            _productsStore = productsStore;
        }

        public ICountContext GetCountContext()
        {
            return new CountContext(_apiClient, _emailService, _environmentsStore, _productsStore, _establishmentCountService);
        }

        public class CountContext : ICountContext
        {
            private readonly CountApiClient _apiClient;
            private readonly IEmailService _emailService;
            private readonly IProductsStore _productsStore;
            private readonly IEnvironmentsStore _environmentsStore;
            private readonly EstablishmentCountService _establishmentCountService;

            private readonly Lazy<Task<Dictionary<int, string>>> _environmentTargetPerId;
            private readonly Lazy<Task<List<Product>>> _products;
            private readonly ConcurrentDictionary<RemoteCountTarget, List<RemoteCountDetail>> _remoteDetails;

            public CountContext
            (
                CountApiClient apiClient,
                IEmailService emailService,
                IEnvironmentsStore environmentsStore,
                IProductsStore productsStore,
                EstablishmentCountService establishmentCountService
            )
            {
                _apiClient = apiClient;
                _emailService = emailService;
                _environmentsStore = environmentsStore;
                _productsStore = productsStore;
                _establishmentCountService = establishmentCountService;

                _environmentTargetPerId = new Lazy<Task<Dictionary<int, string>>>(GetEnvironmentTargetsPerIdAsync);
                _products = new Lazy<Task<List<Product>>>(GetProductsAsync);
                _remoteDetails = new ConcurrentDictionary<RemoteCountTarget, List<RemoteCountDetail>>();
            }

            private Task<List<Product>> GetProductsAsync()
            {
                return _productsStore.GetAsync(ProductsFilter.All, new ProductsIncludes());
            }

            private async Task<Dictionary<int, string>> GetEnvironmentTargetsPerIdAsync()
            {
                var environments = await _environmentsStore.GetAsync(EnvironmentAccessRight.Everything, new EnvironmentFilter
                {
                    Purposes = new HashSet<EnvironmentPurpose> { EnvironmentPurpose.Contractual },
                });
                return environments.ToDictionary(e => e.Id, e => e.ProductionHost);
            }

            public void Dispose()
            {
                var cache = _remoteDetails.ToList();
                _remoteDetails.Clear();
                var cacheAsJson = Serializer.Serialize(cache); // TODO email attachment
                _emailService.SendAsync
                (
                    RecipientForm.FromContact(EmailContact.CloudControl),
                    new EmailContent("Données d'api reçues pendant les décomptes", CountCacheEmail.Template())
                );
            }

            public async Task<ContractWithCountNumber> GetNumberFromRemoteAsync(Contract contract, AccountingPeriod period)
            {
                var target = new RemoteCountTarget
                {
                    ProductId = contract.CommercialOffer.ProductId,
                    BillingMode = contract.CommercialOffer.BillingMode,
                    EnvironmentId = contract.EnvironmentId ?? throw new ApplicationException($"Contract {contract.Id} is not attached to an environment"),
                    EstablishmentIds = _establishmentCountService.GetForPeriod(contract, period).Select(e => e.EstablishmentRemoteId).ToList(),
                };

                var remoteCount = await GetRemoteCountAsync(target, period);
                var details = remoteCount
                    .Select(c => new CountDetail { Number = c.Value, EstablishmentId = c.LegalEntityId })
                    .ToList();

                var filteredDetails = _establishmentCountService.FilterFreeMonths(contract, details, period);

                return new ContractWithCountNumber
                {
                    Contract = contract,
                    Period = period,
                    Details = filteredDetails,
                    CountNumber = filteredDetails.Sum(c => c.Number),
                    CountNumberWithFreeMonths = details.Sum(c => c.Number),
                };
            }

            private async Task<List<RemoteCountDetail>> GetRemoteCountAsync(RemoteCountTarget target, AccountingPeriod period)
            {
                if (_remoteDetails.TryGetValue(target, out var cached))
                {
                    return cached;
                }

                var countDetails = await GetRemoteCountDetailsAsync(target, period);

                _remoteDetails.TryAdd(target, countDetails);
                return countDetails;
            }

            private async Task<List<RemoteCountDetail>> GetRemoteCountDetailsAsync(RemoteCountTarget target, AccountingPeriod period)
            {
                if (!( await _environmentTargetPerId.Value ).TryGetValue(target.EnvironmentId, out var environmentUri))
                {
                    throw new ApplicationException($"Could not find uri of environment {target.EnvironmentId}");
                }

                var products = await _products.Value;
                var remoteDetails = await _apiClient.GetRemoteCountDetailsAsync(environmentUri, target, period, products);
                EnsureExactMatch(target.EstablishmentIds, remoteDetails);
                return remoteDetails;
            }

            private void EnsureExactMatch(List<int> establishmentIds, List<RemoteCountDetail> remoteDetails)
            {
                try
                {
                    if (establishmentIds.Count != remoteDetails.Count)
                    {
                        throw new ApplicationException();
                    }
                    foreach (var establishmentId in establishmentIds)
                    {
                        var a = remoteDetails.Single(d => d.LegalEntityId == establishmentId);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public class RemoteCountTarget : ValueObject
        {
            public int EnvironmentId { get; set; }
            public List<int> EstablishmentIds { get; set; }
            public BillingMode BillingMode { get; set; }
            public int ProductId { get; set; }

            protected override IEnumerable<object> EqualityComponents
            {
                get
                {
                    yield return EnvironmentId;
                    yield return BillingMode;
                    yield return ProductId;
                    foreach (var id in EstablishmentIds.OrderBy(i => i))
                    {
                        yield return id;
                    }
                }
            }
        }
    }
}
