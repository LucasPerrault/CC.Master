using Instances.Domain.Instances;
using Instances.Infra.Dns;
using Instances.Infra.Windows;
using Moq;
using Ovh.Api;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Infra.Tests.Dns
{
    public class DnsServiceTest
    {
        private readonly Mock<IInternalDnsService> _internalDnsServiceMock;
        private readonly Mock<IExternalDnsService> _externalDnsServiceMock;

        public DnsServiceTest()
        {
            _internalDnsServiceMock = new Mock<IInternalDnsService>();
            _externalDnsServiceMock = new Mock<IExternalDnsService>();
        }

        #region CreateAsync
        [Fact]
        public async Task CreateAsync_CallsInternalAndExternalDeleteAndThenInternalAndExternalCreate()
        {
            var calls = new List<string>();
            var deleteInternalTag = "delete-internal";
            var deleteExternalTag = "delete-external";
            var createInternalTag = "create-internal";
            var createExternalTag = "create-external";
            _internalDnsServiceMock.Setup(d => d.DeleteCname(It.IsAny<DnsEntryDeletion>())).Callback(() => calls.Add(deleteInternalTag));
            _externalDnsServiceMock.Setup(d => d.DeleteCnameAsync(It.IsAny<DnsEntryDeletion>())).Returns(Task.CompletedTask).Callback(() => calls.Add(deleteExternalTag));
            _internalDnsServiceMock.Setup(d => d.AddNewCname(It.IsAny<DnsEntryCreation>())).Callback(() => calls.Add(createInternalTag));
            _externalDnsServiceMock.Setup(d => d.AddNewCnameAsync(It.IsAny<DnsEntryCreation>())).Returns(Task.CompletedTask).Callback(() => calls.Add(createExternalTag));
            var dnsService = new DnsService(_internalDnsServiceMock.Object, _externalDnsServiceMock.Object, new DnsZonesConfiguration
            {
                Demos = "ilucca-demo.net"
            });

            await dnsService.CreateAsync(DnsEntry.ForDemo("des-maux", "demo"));

            _internalDnsServiceMock.Verify(d => d.DeleteCname(It.IsAny<DnsEntryDeletion>()), Times.Once);
            _externalDnsServiceMock.Verify(d => d.DeleteCnameAsync(It.IsAny<DnsEntryDeletion>()), Times.Once);
            _internalDnsServiceMock.Verify(d => d.AddNewCname(It.IsAny<DnsEntryCreation>()), Times.Once);
            _externalDnsServiceMock.Verify(d => d.AddNewCnameAsync(It.IsAny<DnsEntryCreation>()), Times.Once);
            Assert.True(calls.IndexOf(deleteInternalTag) < calls.IndexOf(createInternalTag));
            Assert.True(calls.IndexOf(deleteExternalTag) < calls.IndexOf(createExternalTag));
        }

        [Fact]
        public async Task CreateAsync_CallsInternalAndExternalDeleteWithCorrectInformation()
        {
            _internalDnsServiceMock.Setup(d => d.DeleteCname(It.IsAny<DnsEntryDeletion>()));
            _externalDnsServiceMock.Setup(d => d.DeleteCnameAsync(It.IsAny<DnsEntryDeletion>())).Returns(Task.CompletedTask);

            var dnsZoneConfiguration = new DnsZonesConfiguration
            {
                Demos = "ilucca-demo.net"
            };
            var dnsService = new DnsService(_internalDnsServiceMock.Object, _externalDnsServiceMock.Object, dnsZoneConfiguration);

            var dnsEntry = DnsEntry.ForDemo("des-maux", "demo");
            await dnsService.CreateAsync(dnsEntry);

            _internalDnsServiceMock.Verify(d => d.DeleteCname(It.Is<DnsEntryDeletion>(ded => ded.Subdomain == dnsEntry.Subdomain && ded.DnsZone == dnsZoneConfiguration.Demos)), Times.Once);
            _externalDnsServiceMock.Verify(d => d.DeleteCnameAsync(It.Is<DnsEntryDeletion>(ded => ded.Subdomain == dnsEntry.Subdomain && ded.DnsZone == dnsZoneConfiguration.Demos)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_CallsInternalAndExternalCreateWithCorrectInformation()
        {
            _internalDnsServiceMock.Setup(d => d.AddNewCname(It.IsAny<DnsEntryCreation>()));
            _externalDnsServiceMock.Setup(d => d.AddNewCnameAsync(It.IsAny<DnsEntryCreation>())).Returns(Task.CompletedTask);

            var dnsZoneConfiguration = new DnsZonesConfiguration
            {
                Demos = "ilucca-demo.net"
            };
            var dnsService = new DnsService(_internalDnsServiceMock.Object, _externalDnsServiceMock.Object, dnsZoneConfiguration);

            var dnsEntry = DnsEntry.ForDemo("des-maux", "demo");
            await dnsService.CreateAsync(dnsEntry);

            _internalDnsServiceMock.Verify(d => d.AddNewCname(It.Is<DnsEntryCreation>(dec => dec.Subdomain == dnsEntry.Subdomain && dec.DnsZone == dnsZoneConfiguration.Demos && dec.Cluster == dnsEntry.Cluster)), Times.Once);
            _externalDnsServiceMock.Verify(d => d.AddNewCnameAsync(It.Is<DnsEntryCreation>(dec => dec.Subdomain == dnsEntry.Subdomain && dec.DnsZone == dnsZoneConfiguration.Demos && dec.Cluster == dnsEntry.Cluster)), Times.Once);
        }
        #endregion

        #region DeleteAsync
        [Fact]
        public async Task DeleteAsync_CallsInternalAndExternalDelete()
        {
            _internalDnsServiceMock.Setup(d => d.AddNewCname(It.IsAny<DnsEntryCreation>()));
            _externalDnsServiceMock.Setup(d => d.AddNewCnameAsync(It.IsAny<DnsEntryCreation>())).Returns(Task.CompletedTask);
            var dnsService = new DnsService(_internalDnsServiceMock.Object, _externalDnsServiceMock.Object, new DnsZonesConfiguration
            {
                Demos = "ilucca-demo.net"
            });

            await dnsService.DeleteAsync(DnsEntry.ForDemo("des-maux", "demo"));

            _internalDnsServiceMock.Verify(d => d.DeleteCname(It.IsAny<DnsEntryDeletion>()), Times.Once);
            _externalDnsServiceMock.Verify(d => d.DeleteCnameAsync(It.IsAny<DnsEntryDeletion>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsInternalAndExternalDeleteWithCorrectInformation()
        {
            _internalDnsServiceMock.Setup(d => d.DeleteCname(It.IsAny<DnsEntryDeletion>()));
            _externalDnsServiceMock.Setup(d => d.DeleteCnameAsync(It.IsAny<DnsEntryDeletion>())).Returns(Task.CompletedTask);

            var dnsZoneConfiguration = new DnsZonesConfiguration
            {
                Demos = "ilucca-demo.net"
            };
            var dnsService = new DnsService(_internalDnsServiceMock.Object, _externalDnsServiceMock.Object, dnsZoneConfiguration);

            var dnsEntry = DnsEntry.ForDemo("des-maux", "demo");
            await dnsService.DeleteAsync(dnsEntry);

            _internalDnsServiceMock.Verify(d => d.DeleteCname(It.Is<DnsEntryDeletion>(ded => ded.Subdomain == dnsEntry.Subdomain && ded.DnsZone == dnsZoneConfiguration.Demos)), Times.Once);
            _externalDnsServiceMock.Verify(d => d.DeleteCnameAsync(It.Is<DnsEntryDeletion>(ded => ded.Subdomain == dnsEntry.Subdomain && ded.DnsZone == dnsZoneConfiguration.Demos)), Times.Once);
        }

        #endregion

    }
}
