using Instances.Infra.Shared;
using Instances.Infra.Windows;
using System;
using System.Collections.Generic;

namespace Instances.Infra.Dns
{
    public class InternalDnsConfiguration
    {
        public string Server { get; set; }
    }

    public class InternalDnsService
    {
        private readonly Lazy<IWmiSessionWrapper> _session;
        private readonly string _server;
        private readonly IWmiWrapper _wmiWrapper;

        public InternalDnsService(InternalDnsConfiguration configuration, IWmiWrapper wmiWrapper)
        {
            _server = configuration.Server;
            _wmiWrapper = wmiWrapper;
            _session = new Lazy<IWmiSessionWrapper>
            (
                () => _wmiWrapper.CreateSession(@$"\\{_server}\root\microsoftdns")
            );
        }

        internal void AddNewCname(DnsEntryCreation entryCreation)
        {
            _wmiWrapper.InvokeClassMethod(_session.Value, "MicrosoftDNS_CNAMETYPE", "CreateInstanceFromPropertyData", new Dictionary<string, object>
            {
                ["DnsServerName"] = _server,
                ["ContainerName"] = entryCreation.DnsZone,
                ["OwnerName"] = $"{entryCreation.Subdomain}.{entryCreation.DnsZone}",
                ["PrimaryName"] = GetPrimaryName(entryCreation.Cluster),
            });
        }

        internal  void DeleteCname(DnsEntryDeletion deletion)
        {
            _wmiWrapper.QueryAndDeleteObjects(_session.Value, $"SELECT * FROM MicrosoftDNS_CNAMETYPE WHERE OwnerName = '{deletion.Subdomain}.{deletion.DnsZone}'");
        }

        private string GetPrimaryName(string targetClusterName)
        {
            return $"rbx-{ClusterNameConvertor.GetShortName(targetClusterName)}-haproxy.lucca.local.";
        }
    }
}
