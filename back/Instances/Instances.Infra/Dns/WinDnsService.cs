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
        private Lazy<IWmiSessionWrapper> _session;
        private readonly string _server;
        private readonly IWmiWrapper _wmiWrapper;

        public WinDnsService(WinDnsConfiguration configuration, IWmiWrapper wmiWrapper)
        {
            _server = configuration.Server;
            _wmiWrapper = wmiWrapper;
            _session = GetNewLazySession();
        }

        public void AddNewCname(DnsEntryCreation entryCreation)
        {
            SafeRun(() => UnsafeAddNewCname(entryCreation));
        }

        public void DeleteCname(DnsEntryDeletion entryDeletion)
        {

            SafeRun(() => UnsafeDeleteCname(entryDeletion));
        }

        private void UnsafeAddNewCname(DnsEntryCreation entryCreation)
        {
            _wmiWrapper.InvokeClassMethod(_session.Value, WmiConstants.ClassCNameType, WmiConstants.MethodCreateInstanceFromPropertyData, new Dictionary<string, object>
            {
                [WmiConstants.PropertyCNameTypeDnsServerName] = _server,
                [WmiConstants.PropertyCNameTypeContainerName] = entryCreation.DnsZone,
                [WmiConstants.PropertyCNameTypeOwnerName] = $"{entryCreation.Subdomain}.{entryCreation.DnsZone}",
                [WmiConstants.PropertyCNameTypePrimaryName] = GetPrimaryName(entryCreation.Cluster),
            });
        }

        private void UnsafeDeleteCname(DnsEntryDeletion entryDeletion)
        {
            _wmiWrapper.QueryAndDeleteObjects(_session.Value, $"SELECT * FROM {WmiConstants.ClassCNameType} WHERE {WmiConstants.PropertyCNameTypeOwnerName} = '{entryDeletion.Subdomain}.{entryDeletion.DnsZone}'");
        }

        private void SafeRun(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                _session = GetNewLazySession();
                throw;
            }
        }

        private Lazy<IWmiSessionWrapper> GetNewLazySession()
        {
            return new Lazy<IWmiSessionWrapper>
            (
                () => _wmiWrapper.CreateSession(@$"\\{_server}\root\microsoftdns")
            );
        }

        private string GetPrimaryName(string targetClusterName)
        {
            return $"rbx-{ClusterNameConvertor.GetShortName(targetClusterName)}-haproxy.lucca.local.";
        }
    }
}
