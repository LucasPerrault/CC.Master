using Cache.Abstractions;
using Instances.Application.Demos.Duplication;
using Instances.Application.Instances;
using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Shared;
using Moq;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Application.Tests.Demos
{
    public class HubspotDuplicatorTests
    {
        private readonly Mock<ICacheService> _cacheService = new Mock<ICacheService>();
        private readonly Mock<IHubspotService> _hubspotServiceMock = new Mock<IHubspotService>();
        private readonly Mock<IDemosStore> _demosStoreMock = new Mock<IDemosStore>();
        private readonly Mock<ISubdomainGenerator> _subdomainGeneratorMock = new Mock<ISubdomainGenerator>();
        private readonly Mock<IClusterSelector> _clusterSelectorMock = new Mock<IClusterSelector>();
        private readonly Mock<IDemoDuplicationsStore> _demoDuplicationsStore = new Mock<IDemoDuplicationsStore>();

        private readonly Mock<ISqlScriptPicker> _sqlScriptPickerMock = new Mock<ISqlScriptPicker>();
        private readonly Mock<ICcDataService> _ccDataServiceMock = new Mock<ICcDataService>();
        private readonly Mock<IDnsService> _dnsServiceMock = new Mock<IDnsService>();

        [Fact]
        public async Task ShouldUpdateSubdomainOnHubspot()
        {
            var duplicator = GetDuplicator();

            await duplicator.DuplicateMasterForHubspotAsync
                (
                    new HubspotDemoDuplication
                    {
                        VId = 1,
                        FailureWorkflowId = 666,
                        SuccessWorkflowId = 777
                    }
                );

            _hubspotServiceMock.Verify(s => s.UpdateContactSubdomainAsync(1, "aperture-science"));
        }

        [Fact]
        public async Task ShouldCacheDuplication()
        {
            var duplicator = GetDuplicator();

            await duplicator.DuplicateMasterForHubspotAsync
                (
                    new HubspotDemoDuplication
                    {
                        VId = 1,
                        FailureWorkflowId = 666,
                        SuccessWorkflowId = 777
                    }
                );

            _cacheService.Verify(s => s.SetAsync(
                    It.IsAny<HubspotDemoDuplicationKey>(),
                    It.Is<HubspotCachedDuplication>(d =>
                        d.FailureWorkflowId == 666
                        && d.SuccessWorkflowId == 777
                        && d.Email == "glados@aperture-science.com"),
                    ItExpiresAfter(TimeSpan.FromMinutes(20))
                ), Times.Once);
        }


        [Fact]
        public async Task ShouldTriggerDuplicationOnRemote()
        {
            var duplicator = GetDuplicator();

            await duplicator.DuplicateMasterForHubspotAsync
                (
                    new HubspotDemoDuplication
                    {
                        VId = 1,
                        FailureWorkflowId = 666,
                        SuccessWorkflowId = 777
                    }
                );

            _ccDataServiceMock.Verify(s => s.StartDuplicateInstanceAsync(
                    It.Is<DuplicateInstanceRequestDto>(r =>
                        r.SourceTenant.Tenant == "mocked-master-demo"
                        && r.TargetTenant == "aperture-science"),
                    "demo-cluster",
                    It.IsAny<string>()
                ));
        }


        [Fact]
        public async Task ShouldAddDns()
        {
            _dnsServiceMock.Setup(d => d.CreateAsync(It.IsAny<DnsEntry>())).Returns(Task.CompletedTask);
            var duplicator = GetDuplicator();

            await duplicator.DuplicateMasterForHubspotAsync
                (
                    new HubspotDemoDuplication
                    {
                        VId = 1,
                        FailureWorkflowId = 666,
                        SuccessWorkflowId = 777
                    }
                );

            _dnsServiceMock.Verify(d => d.CreateAsync(It.IsAny<DnsEntry>()));
        }

        private static DurationCacheInvalidation ItExpiresAfter(TimeSpan timeSpan)
        {
            return It.Is<DurationCacheInvalidation>(i => i.Duration == timeSpan);
        }

        private HubspotDemoDuplicator GetDuplicator()
        {
            _subdomainGeneratorMock.Setup(g => g.GetSubdomainFromPrefixAsync(It.IsAny<string>()))
                .ReturnsAsync("aperture-science");

            _hubspotServiceMock.Setup(s => s.GetContactAsync(1))
                .ReturnsAsync
                    (
                        new HubspotContact
                        {
                            VId = 1,
                            Email = "glados@aperture-science.com",
                            IpAddress = "127.0.0.1",
                            Company = "Aperture Science"
                        }
                    );

            _demosStoreMock.Setup(s => s.GetActiveByIdAsync(It.IsAny<int>(), It.IsAny<AccessRight>()))
                .ReturnsAsync
                    (
                        new Demo
                        {
                            Id = 123,
                            Cluster = "mocked-master-demo-cluster",
                            Subdomain = "mocked-master-demo",
                            Instance = new Instance {  }
                        }
                    );

            _clusterSelectorMock.Setup(s => s.GetFillingClusterAsync("aperture-science"))
                .ReturnsAsync("demo-cluster");

            _sqlScriptPickerMock.Setup(p => p.GetForDuplication(It.IsAny<InstanceDuplication>()))
                .Returns(new List<Uri>());

            var duplicator = new HubspotDemoDuplicator
                (
                    _cacheService.Object,
                    _hubspotServiceMock.Object,
                    _demosStoreMock.Object,
                    _subdomainGeneratorMock.Object,
                    _clusterSelectorMock.Object,
                    _demoDuplicationsStore.Object,
                    new InstancesManipulator
                        (
                            _sqlScriptPickerMock.Object,
                            _ccDataServiceMock.Object
                        ),
                    _dnsServiceMock.Object
                );
            return duplicator;
        }
    }
}
