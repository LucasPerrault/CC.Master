using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Instance.Filters;
using AdvancedFilters.Domain.Instance.Interfaces;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Services.Sync;
using AdvancedFilters.Infra.Services.Sync.Dtos;
using AdvancedFilters.Infra.Storage.Services;
using AdvancedFilters.Web;
using AdvancedFilters.Web.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Remote.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Testing.Infra;
using Xunit;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Tests
{
    public class HugeSyncServiceTests
    {
        private readonly Mock<IEnvironmentsStore> _environmentsStoreMock;
        private readonly Mock<IBulkUpsertService> _upsertServiceMock;
        private readonly IDataSourceSyncCreationService _creationService;
        private readonly Mock<HttpClientHandler> _httpClientHandlerMock;
        private readonly Mock<ILogger<IDataSourceSynchronizer>> _loggerMock;

        public HugeSyncServiceTests()
        {
            _httpClientHandlerMock = new Mock<HttpClientHandler>(MockBehavior.Strict);
            _upsertServiceMock = new Mock<IBulkUpsertService>();
            _environmentsStoreMock = new Mock<IEnvironmentsStore>();

            var client = new HttpClient(_httpClientHandlerMock.Object);
            var localDataSourceService = new Mock<ILocalDataSourceService>().Object;
            _loggerMock = new Mock<ILogger<IDataSourceSynchronizer>>();
            _creationService = new DataSourceSyncCreationService
            (
                client,
                _upsertServiceMock.Object,
                new HttpConfiguration { MaxParallelCalls = 42 },
                new FetchAuthenticator(),
                _loggerMock.Object,
                localDataSourceService
            );
        }

        [Fact]
        public async Task ShouldSyncEnvironments()
        {
            SetupKnownEnvironments();

            var confs = new Dictionary<DataSources, DataSource>
            {
                [DataSources.Environments] = DataSourceMapper.Get(DataSources.Environments, Configuration)
            };
            var service = new SyncService(new DataSourcesRepository(confs), _creationService, _environmentsStoreMock.Object);

            SetupHttpResponse("https://mocked-cc.ilucca.local/api/envs", new EnvironmentsDto
            {
                Items = new List<Environment> { new Environment { Subdomain = "aperture-science", Id = 1} }
            });

            await service.SyncEverythingAsync();
            _upsertServiceMock.Verify
            (
                s => s.InsertOrUpdateOrDeleteAsync
                (
                    It.Is<IReadOnlyCollection<Environment>>(collection => collection.Single().Subdomain == "aperture-science"),
                    It.Is<BulkUpsertConfig>(c => c.IncludeSubEntities == false)
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task ShouldSyncAppInstances()
        {

            var confs = new Dictionary<DataSources, DataSource>
            {
                [DataSources.AppInstances] = DataSourceMapper.Get(DataSources.AppInstances, Configuration)
            };
            var service = new SyncService(new DataSourcesRepository(confs), _creationService, _environmentsStoreMock.Object);

            SetupKnownEnvironments(new Environment { Id = 42, ProductionHost = "https://mocked-tenant.dev" });
            SetupHttpResponse("https://mocked-tenant.dev/api/app-instances", new AppInstancesDto
            {
                Data = ApiV3Response(new AppInstance { EnvironmentId = 42, ApplicationId = "glados" })
            });

            await service.SyncEverythingAsync();
            _upsertServiceMock.Verify
            (
                s => s.InsertOrUpdateOrDeleteAsync
                (
                    It.Is<IReadOnlyCollection<AppInstance>>(collection => collection.Single().ApplicationId == "glados"),
                    It.Is<BulkUpsertConfig>(c => c.IncludeSubEntities == false)
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task ShouldSyncLegalUnits()
        {

            var confs = new Dictionary<DataSources, DataSource>
            {
                [DataSources.LegalUnits] = DataSourceMapper.Get(DataSources.LegalUnits, Configuration)
            };
            var service = new SyncService(new DataSourcesRepository(confs), _creationService, _environmentsStoreMock.Object);

            SetupKnownEnvironments(new Environment { Id = 42, ProductionHost = "https://mocked-tenant.dev" });
            SetupHttpResponse("https://mocked-tenant.dev/api/legal-units", new LegalUnitsDto
            {
                Items = new List<LegalUnit> { new LegalUnit { EnvironmentId = 42, Name = "Aperture Science Colorado"} }
            });

            await service.SyncEverythingAsync();
            _upsertServiceMock.Verify
            (
                s => s.InsertOrUpdateOrDeleteAsync
                (
                    It.Is<IReadOnlyCollection<LegalUnit>>(collection => collection.Single().Name == "Aperture Science Colorado"),
                    It.Is<BulkUpsertConfig>(c => c.IncludeSubEntities == false)
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task ShouldSkipMissedTargets()
        {

            var confs = new Dictionary<DataSources, DataSource>
            {
                [DataSources.AppInstances] = DataSourceMapper.Get(DataSources.AppInstances, Configuration),
                [DataSources.LegalUnits] = DataSourceMapper.Get(DataSources.LegalUnits, Configuration),
            };
            var service = new SyncService(new DataSourcesRepository(confs), _creationService, _environmentsStoreMock.Object);

            SetupKnownEnvironments
            (
                new Environment { Id = 1, Subdomain = "toto", ProductionHost = "https://toto.dev" },
                new Environment { Id = 2, Subdomain = "titi", ProductionHost = "https://titi.dev" }
            );
            SetupHttpResponse("https://toto.dev/api/legal-units", new LegalUnitsDto
            {
                Items = new List<LegalUnit> { new LegalUnit { EnvironmentId = 1 } }
            });
            SetupHttpResponse("https://toto.dev/api/app-instances", new AppInstancesDto
            {
                Data = ApiV3Response(new AppInstance { EnvironmentId = 1 })
            });

            await service.SyncMonoTenantDataAsync(new HashSet<string> { "toto", "titi" });
            _httpClientHandlerMock.VerifySendAsync(MessageWithUrl(HttpMethod.Get, "https://toto.dev/api/app-instances"), Times.Once());
            _httpClientHandlerMock.VerifySendAsync(MessageWithUrl(HttpMethod.Get, "https://titi.dev/api/app-instances"), Times.Once());
            _httpClientHandlerMock.VerifySendAsync(MessageWithUrl(HttpMethod.Get, "https://toto.dev/api/legal-units"), Times.Once());
            _httpClientHandlerMock.VerifySendAsync(MessageWithUrl(HttpMethod.Get, "https://titi.dev/api/legal-units"), Times.Never());
        }

        private ApiV3DtoData<T> ApiV3Response<T>(params T[] elements)
        {
            var list = elements == null ? new List<T>() : elements.ToList();
            return new ApiV3DtoData<T> { Items = list };
        }

        private ItIsRequestMessage MessageWithUrl(HttpMethod method, string url)
        {
            return ItIsRequestMessage.Matching(m => m.RequestUri.ToString() == url && m.Method == method);
        }

        private void SetupHttpResponse<TDto>(string url, TDto dto)
        {
            _httpClientHandlerMock
                .SetupSendAsync(MessageWithUrl(HttpMethod.Get, url))
                .ReturnsAsync(new HttpResponseMessage { Content = dto.ToJsonPayload() });
        }

        private void SetupKnownEnvironments(params Environment[] environments)
        {
            var envs = environments == null ? new List<Environment>() : environments.ToList();
            _environmentsStoreMock.Setup(s => s.GetAsync(It.IsAny<EnvironmentFilter>())).ReturnsAsync(envs);
        }

        private static readonly AdvancedFiltersConfiguration Configuration = new AdvancedFiltersConfiguration
        {
            Auth = new AuthenticationConfiguration
            {
                MonolithWebserviceToken = Guid.Empty,
                ClientCenterWebserviceToken = Guid.Empty,
                CloudControlAuthParameter = "application=a_guid",
                OrganizationStructureWebserviceToken = Guid.Empty,
                CloudControlAuthScheme = "cloudcontrol"
            },
            Routes = new RoutesConfiguration
            {
                Hosts = new HostRoutesConfiguration
                {
                    CloudControl = new CloudControlHostConfiguration
                    {
                        Host = new Uri("https://mocked-cc.ilucca.local"),
                        ClientsEndpoint = "api/clients",
                        ContractsEndpoint = "api/contracts",
                        EnvironmentsEndpoint = "api/envs",
                        ContractsSubdomainParamName = "ContractsSubdomainParamName"
                    },
                },
                Tenants = new TenantRoutesConfiguration
                {
                    EstablishmentsEndpoint = "api/establishments",
                    AppContactsEndpoint = "api/app-contacts",
                    AppInstancesEndpoint = "api/app-instances",
                    ClientContactsEndpoint = "api/client-contacts",
                    LegalUnitsEndpoint = "api/legal-units",
                    SpecializedContactsEndpoint = "api/specialized-contacts"
                }
            }
        };
    }
}
