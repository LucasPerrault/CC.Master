using Instances.Domain.Instances;
using Instances.Infra.Shared;
using Instances.Infra.Windows;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<WinDnsService> _logger;

        public WinDnsService(
            WinDnsConfiguration configuration, IWmiWrapper wmiWrapper,
            ILogger<WinDnsService> logger)
        {
            _server = configuration.Server;
            _wmiWrapper = wmiWrapper;
            _logger = logger;
            _session = new LazyWithReset<IWmiSessionWrapper>
            (
                () => _wmiWrapper.CreateSession(@$"\\{_server}\root\microsoftdns")
            );
        }

        public void AddNewCname(DnsEntryCreation entryCreation)
        {
            var target = GetPrimaryName(entryCreation.Cluster);
            _logger.LogDebug("Create new CNAME on win : {from} ({zone}) to {target} ", entryCreation.Subdomain, entryCreation.DnsZone, target);
            _session.SafeRun(LazyWithResetRetry.Once, session =>
            {
                _wmiWrapper.InvokeClassMethod(session, WmiConstants.ClassCNameType, WmiConstants.MethodCreateInstanceFromPropertyData, new Dictionary<string, object>
                {
                    [WmiConstants.PropertyCNameTypeDnsServerName] = _server,
                    [WmiConstants.PropertyCNameTypeContainerName] = entryCreation.DnsZone,
                    [WmiConstants.PropertyCNameTypeOwnerName] = $"{entryCreation.Subdomain}.{entryCreation.DnsZone}",
                    [WmiConstants.PropertyCNameTypePrimaryName] = target
                });
            });
        }

        public void DeleteCname(DnsEntryDeletion entryDeletion)
        {
            _logger.LogDebug("Delete CNAME on ovh : {domain} ({zone})", entryDeletion.Subdomain, entryDeletion.DnsZone);
            _session.SafeRun(LazyWithResetRetry.Once, session =>
            {
                _wmiWrapper.QueryAndDeleteObjects(session, $"SELECT * FROM {WmiConstants.ClassCNameType} WHERE {WmiConstants.PropertyCNameTypeOwnerName} = '{entryDeletion.Subdomain}.{entryDeletion.DnsZone}'");
            });
        }

        private string GetPrimaryName(string targetClusterName)
        {
            if (targetClusterName == IDnsService.RedirectionCluster)
            {
                return "lab2.lucca.fr.";
            }
            return $"rbx-{ClusterNameConvertor.GetShortName(targetClusterName)}-haproxy.lucca.local.";
        }
    }
}
