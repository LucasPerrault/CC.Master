using Instances.Infra.Dns;
using Instances.Infra.Shared;
using Instances.Infra.Windows;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Infra.Tests.Dns
{
    public class WinDnsServiceTest
    {
        private readonly Mock<IWmiWrapper> _wmiWrapperMock;
        private readonly Mock<ILogger<WinDnsService>> _loggerMock;

        public WinDnsServiceTest()
        {
            _wmiWrapperMock = new Mock<IWmiWrapper>();
            _loggerMock = new Mock<ILogger<WinDnsService>>();
        }

        #region AddNewCname
        [Fact]
        public void AddNewCname_ShouldCallWmiWithTheCorrectData()
        {
            var internalDnsConfiguration = new WinDnsConfiguration { Server = "my-dns-server" };
            _wmiWrapperMock.Setup(w => w.InvokeClassMethod(It.IsAny<WmiSessionWrapper>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()));

            var winDnsService = new WinDnsService(internalDnsConfiguration, _wmiWrapperMock.Object, _loggerMock.Object);
            var dnsEntryCreation = new DnsEntryCreation
            {
                Cluster = "demo",
                DnsZone = "my-zone",
                Subdomain = "des-maux",
            };

            winDnsService.AddNewCname(dnsEntryCreation);

            _wmiWrapperMock.Verify(w => w.InvokeClassMethod(
                It.IsAny<WmiSessionWrapper>(),
                It.Is<string>(className => className == WmiConstants.ClassCNameType),
                It.Is<string>(method => method == WmiConstants.MethodCreateInstanceFromPropertyData),
                It.Is<Dictionary<string, object>>(d =>
                    d.ContainsKey(WmiConstants.PropertyCNameTypeContainerName)
                    && (string)d[WmiConstants.PropertyCNameTypeContainerName] == dnsEntryCreation.DnsZone
                    && d.ContainsKey(WmiConstants.PropertyCNameTypeDnsServerName)
                    && (string)d[WmiConstants.PropertyCNameTypeDnsServerName] == internalDnsConfiguration.Server
                    && d.ContainsKey(WmiConstants.PropertyCNameTypeOwnerName)
                     && (string)d[WmiConstants.PropertyCNameTypeOwnerName] == $"{dnsEntryCreation.Subdomain}.{dnsEntryCreation.DnsZone}"
                    && d.ContainsKey(WmiConstants.PropertyCNameTypePrimaryName)
                     && (string)d[WmiConstants.PropertyCNameTypePrimaryName] == $"rbx-{ClusterNameConvertor.GetShortName(dnsEntryCreation.Cluster)}-haproxy.lucca.local."
                )), Times.Once
            );
        }

        [Fact]
        public void AddNewCname_ShouldConnectToTheCorrectPath()
        {
            var internalDnsConfiguration = new WinDnsConfiguration { Server = "my-dns-server" };
            _wmiWrapperMock.Setup(w => w.CreateSession(It.IsAny<string>()));

            var winDnsService = new WinDnsService(internalDnsConfiguration, _wmiWrapperMock.Object, _loggerMock.Object);
            var dnsEntryCreation = new DnsEntryCreation
            {
                Cluster = "demo",
                DnsZone = "my-zone",
                Subdomain = "des-maux",
            };

            winDnsService.AddNewCname(dnsEntryCreation);

            _wmiWrapperMock.Verify(w => w.CreateSession(It.Is<string>(sessionPath => sessionPath == @$"\\{internalDnsConfiguration.Server}\root\microsoftdns")), Times.Once);
        }

        #endregion

        #region DeleteCname
        [Fact]
        public void DeleteCname_ShouldQueryUsingTheCorrectData()
        {
            var internalDnsConfiguration = new WinDnsConfiguration { Server = "my-dns-server" };
            _wmiWrapperMock.Setup(w => w.QueryAndDeleteObjects(It.IsAny<IWmiSessionWrapper>(), It.IsAny<string>()));

            var winDnsService = new WinDnsService(internalDnsConfiguration, _wmiWrapperMock.Object, _loggerMock.Object);
            var dnsEntryDeletion = new DnsEntryDeletion
            {
                DnsZone = "my-zone",
                Subdomain = "des-maux",
            };

            winDnsService.DeleteCname(dnsEntryDeletion);


            _wmiWrapperMock.Verify(w => w.QueryAndDeleteObjects(It.IsAny<IWmiSessionWrapper>(),
                It.Is<string>(q => q.Contains(WmiConstants.ClassCNameType)
                    && q.Contains(WmiConstants.PropertyCNameTypeOwnerName)
                    && q.Contains($"{dnsEntryDeletion.Subdomain}.{dnsEntryDeletion.DnsZone}"))
                ), Times.Once);
        }

        [Fact]
        public void DeleteCname_ShouldConnectToTheCorrectPath()
        {
            var internalDnsConfiguration = new WinDnsConfiguration { Server = "my-dns-server" };
            _wmiWrapperMock.Setup(w => w.CreateSession(It.IsAny<string>()));

            var winDnsService = new WinDnsService(internalDnsConfiguration, _wmiWrapperMock.Object, _loggerMock.Object);
            var dnsEntryDeletion = new DnsEntryDeletion
            {
                DnsZone = "my-zone",
                Subdomain = "des-maux",
            };

            winDnsService.DeleteCname(dnsEntryDeletion);

            _wmiWrapperMock.Verify(w => w.CreateSession(It.Is<string>(sessionPath => sessionPath == @$"\\{internalDnsConfiguration.Server}\root\microsoftdns")), Times.Once);
        }

        #endregion

    }
}
