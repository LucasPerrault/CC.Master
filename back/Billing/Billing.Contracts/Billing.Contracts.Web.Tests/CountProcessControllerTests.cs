using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Billing.Contracts.Domain.Counts;
using Billing.Contracts.Domain.Counts.Filtering;
using Billing.Contracts.Domain.Counts.Interfaces;
using Billing.Contracts.Domain.Environments;
using Billing.Contracts.Domain.Offers;
using Billing.Contracts.Domain.Offers.Interfaces;
using Billing.Contracts.Infra.Configurations;
using Billing.Contracts.Infra.Services;
using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using CloudControl.Web.Tests.Mocks;
using Core.Proxy.Infra.Configuration;
using Email.Domain;
using Environments.Application;
using Environments.Domain;
using Environments.Domain.Storage;
using Environments.Infra.Storage;
using Environments.Web;
using FluentAssertions;
using Lock;
using Moq;
using Remote.Infra.Extensions;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TeamNotification.Abstractions;
using Testing.Infra;
using Tools;
using Xunit;
using Environment = Environments.Domain.Environment;

namespace Billing.Contracts.Web.Tests
{
    public class CountProcessControllerTests
    {
        [Fact]
        public async Task ShouldRunForOneContract()
        {
            var mockedResponse = new CountApiClient.CountRemoteApiContainer
            {
                Data = new CountApiClient.CountRemoteApiDtoList
                {
                    Items = new List<RemoteCountDetail>
                    {
                        new RemoteCountDetail { Value = 20, LegalEntityId = 1 },
                    },
                },
            };

            var remoteCountHandlerMock = new Mock<HttpClientHandler>();
            remoteCountHandlerMock
                .SetupSendAsync(ItIsRequestMessage.Any())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = mockedResponse.ToJsonPayload(),
                });

            var apiClient = new CountApiClient(new HttpClient(remoteCountHandlerMock.Object));

            var contract = new Contract
            {
                TheoreticalStartOn = new DateTime(2020, 01, 01),
                TheoreticalEndOn = new DateTime(2020, 12, 30),
                ArchivedAt = new DateTime(2024, 01, 01),
                EnvironmentId = 10,
                Environment = new ContractEnvironment { GroupId = 1 },
                Attachments = new List<EstablishmentAttachment>
                {
                    new EstablishmentAttachment
                    {
                        EstablishmentRemoteId = 1,
                        StartsOn = new DateTime(2020, 01, 01),
                        EndsOn = new DateTime(2020, 01, 31)
                    },
                },
                CommercialOffer = new CommercialOffer
                {
                    ProductId = 10,
                    Product = new Product { Id = 10 },
                    PricingMethod = PricingMethod.Linear,
                    PriceLists = new List<PriceList>
                    {
                        new PriceList
                        {
                            StartsOn = new DateTime(2020, 01, 01),
                            Rows = new List<PriceRow>
                            {
                                new PriceRow { MaxIncludedCount = 30, FixedPrice = 0, UnitPrice = 0.2m },
                            },
                        },
                    },
                },
            };

            var query = new CountProcessesController.CountProcessQuery
            {
                Period = new AccountingPeriod(2020, 01),
            }.ToJsonPayload(new AccountingPeriodJsonConverter());

            var legacyCloudControlConfiguration = new LegacyCloudControlConfiguration
            {
                Host = "cc-legacy.mocked.url",
            };

            var billingConfiguration = new BillingContractsConfiguration
            {
                TenantCountsApiWebServiceToken = Guid.Empty,
            };

            var webApplicationFactory = new MockedWebApplicationFactory();
            webApplicationFactory.Mocks.AddCustomRegister(s =>
            {
                EnvironmentsConfigurer.ConfigureEnvironments(s, new EnvironmentConfiguration
                {
                    LegacyToken = Guid.NewGuid(),
                    LegacyHost = legacyCloudControlConfiguration.Uri,
                    Renaming = new EnvironmentRenamingConfiguration
                    {
                        SlackChannel = "slackchannel"
                    },
                });
                ContractsConfigurer.ConfigureServices(s, legacyCloudControlConfiguration, billingConfiguration);
            });

            var productsStore = new Mock<IProductsStore>();
            productsStore
                .Setup(s => s.GetAsync(It.IsAny<ProductsFilter>(), It.IsAny<ProductsIncludes>()))
                .ReturnsAsync(new List<Product> { new Product { Id = 10, ApplicationCode = "P10"} });

            var contractsStore = new Mock<IContractsStore>();
            contractsStore
                .Setup(s => s.GetAsync(It.IsAny<AccessRight>(), It.IsAny<ContractFilter>()))
                .ReturnsAsync(new List<Contract> { contract });

            var pricingsStore = new Mock<IContractPricingsStore>();
            pricingsStore
                .Setup(s => s.GetAsync())
                .ReturnsAsync(new List<ContractPricing> {  });

            var countsStore = new Mock<ICountsStore>();
            countsStore
                .Setup(s => s.GetAsync(It.IsAny<AccessRight>(), It.IsAny<CountFilter>()))
                .ReturnsAsync(new List<Count>());

            var envStore = new Mock<IEnvironmentsStore>();
            envStore
                .Setup(s => s.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment>
                {
                    new Environment { Id = 10, Domain = EnvironmentDomain.ILuccaDotNet, Subdomain = "aperture" }
                });

            webApplicationFactory.Mocks.AddSingleton(new Mock<ITeamNotifier>().Object);
            webApplicationFactory.Mocks.AddSingleton(new Mock<IEmailService>().Object);
            webApplicationFactory.Mocks.AddScoped(new Mock<ILockService>().Object);
            webApplicationFactory.Mocks.AddScoped(productsStore.Object);
            webApplicationFactory.Mocks.AddScoped(envStore.Object);
            webApplicationFactory.Mocks.AddScoped(contractsStore.Object);
            webApplicationFactory.Mocks.AddScoped(pricingsStore.Object);
            webApplicationFactory.Mocks.AddScoped(countsStore.Object);
            webApplicationFactory.Mocks.AddScoped(InMemoryDbHelper.InitialiseDb<EnvironmentsDbContext>("envs", o => new EnvironmentsDbContext(o)));
            webApplicationFactory.Mocks.AddTransient(apiClient);

            var httpClient = webApplicationFactory.CreateAuthenticatedClient();
            var response = await httpClient.PostAsync("/api/count-processes/launch", query);
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            }
            var content = await response.Content.ReadAsStreamAsync();
            var result = await Serializer.DeserializeAsync<CountProcessResult>(content, new AccountingPeriodJsonConverter());
            result.ExceptionsPerContractId.Should().BeEmpty();
        }
    }
}
