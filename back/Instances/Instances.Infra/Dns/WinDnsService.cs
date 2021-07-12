using Instances.Infra.Shared;
using Instances.Infra.Windows;
using System;
using System.Collections.Generic;

namespace Instances.Infra.Dns
{
    public class WinDnsConfiguration
    {
        public string Server { get; set; }
    }

    public class WinDnsService : IInternalDnsService
    {
        private readonly Lazy<IWmiSessionWrapper> _session;
        private readonly string _server;
        private readonly IWmiWrapper _wmiWrapper;

        public WinDnsService(WinDnsConfiguration configuration, IWmiWrapper wmiWrapper)
        {
            _server = configuration.Server;
            _wmiWrapper = wmiWrapper;
            _session = new Lazy<IWmiSessionWrapper>
            (
                () => _wmiWrapper.CreateSession(@$"\\{_server}\root\microsoftdns")
            );
        }

        public void AddNewCname(DnsEntryCreation entryCreation)
        {
            _wmiWrapper.InvokeClassMethod(_session.Value, WmiConstants.ClassCNameType, WmiConstants.MethodCreateInstanceFromPropertyData, new Dictionary<string, object>
            {
                [WmiConstants.PropertyCNameTypeDnsServerName] = _server,
                [WmiConstants.PropertyCNameTypeContainerName] = entryCreation.DnsZone,
                [WmiConstants.PropertyCNameTypeOwnerName] = $"{entryCreation.Subdomain}.{entryCreation.DnsZone}",
                [WmiConstants.PropertyCNameTypePrimaryName] = GetPrimaryName(entryCreation.Cluster),
            });
        }

        public void DeleteCname(DnsEntryDeletion entryDeletion)
        {
            _wmiWrapper.QueryAndDeleteObjects(_session.Value, $"SELECT * FROM {WmiConstants.ClassCNameType} WHERE {WmiConstants.PropertyCNameTypeOwnerName} = '{entryDeletion.Subdomain}.{entryDeletion.DnsZone}'");
        }

        private string GetPrimaryName(string targetClusterName)
        {
            return $"rbx-{ClusterNameConvertor.GetShortName(targetClusterName)}-haproxy.lucca.local.";
        }
    }
}
