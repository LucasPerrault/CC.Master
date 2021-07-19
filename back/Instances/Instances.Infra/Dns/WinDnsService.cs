using Instances.Infra.Shared;
using Instances.Infra.Windows;
using System.Collections.Generic;
using Tools;

namespace Instances.Infra.Dns
{
    public class WinDnsConfiguration
    {
        public string Server { get; set; }
    }

    public class WinDnsService : IInternalDnsService
    {
        private readonly LazyWithReset<IWmiSessionWrapper> _session;
        private readonly string _server;
        private readonly IWmiWrapper _wmiWrapper;

        public WinDnsService(WinDnsConfiguration configuration, IWmiWrapper wmiWrapper)
        {
            _server = configuration.Server;
            _wmiWrapper = wmiWrapper;
            _session = new LazyWithReset<IWmiSessionWrapper>
            (
                () => _wmiWrapper.CreateSession(@$"\\{_server}\root\microsoftdns")
            );
        }

        public void AddNewCname(DnsEntryCreation entryCreation)
        {

            _session.SafeRun(session =>
                {
                    _wmiWrapper.InvokeClassMethod(session, WmiConstants.ClassCNameType, WmiConstants.MethodCreateInstanceFromPropertyData, new Dictionary<string, object>
                    {
                        [WmiConstants.PropertyCNameTypeDnsServerName] = _server,
                        [WmiConstants.PropertyCNameTypeContainerName] = entryCreation.DnsZone,
                        [WmiConstants.PropertyCNameTypeOwnerName] = $"{entryCreation.Subdomain}.{entryCreation.DnsZone}",
                        [WmiConstants.PropertyCNameTypePrimaryName] = GetPrimaryName(entryCreation.Cluster),
                    });
                });

        }

        public void DeleteCname(DnsEntryDeletion entryDeletion)
        {
            _session.SafeRun(
                session =>
                {
                    _wmiWrapper.QueryAndDeleteObjects(session, $"SELECT * FROM {WmiConstants.ClassCNameType} WHERE {WmiConstants.PropertyCNameTypeOwnerName} = '{entryDeletion.Subdomain}.{entryDeletion.DnsZone}'");
                });
        }

        private string GetPrimaryName(string targetClusterName)
        {
            return $"rbx-{ClusterNameConvertor.GetShortName(targetClusterName)}-haproxy.lucca.local.";
        }
    }
}
