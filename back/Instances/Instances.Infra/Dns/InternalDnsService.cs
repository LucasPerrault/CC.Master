using Instances.Infra.Shared;
using System;
using System.Management;

namespace Instances.Infra.Dns
{
    public class InternalDnsConfiguration
    {
        public string Server { get; set; }
    }

    public class InternalDnsService
    {
        private readonly Lazy<ManagementScope> _session;
        private readonly string _server;

        public InternalDnsService(InternalDnsConfiguration configuration)
        {
            _server = configuration.Server;

            _session = new Lazy<ManagementScope>
            (
                () =>
                {
                    var session = new ManagementScope(@$"\\{_server}\root\microsoftdns") {
                        Options = new ConnectionOptions { Impersonation = ImpersonationLevel.Impersonate }
                    };
                    session.Connect();
                    return session;
                }
            );
        }

        internal void AddNewCname(DnsEntryCreation entryCreation)
        {
            var man = new ManagementClass(_session.Value, new ManagementPath("MicrosoftDNS_CNAMETYPE"), null);
            var vars = man.GetMethodParameters("CreateInstanceFromPropertyData");
            vars["DnsServerName"] = _server;
            vars["ContainerName"] = entryCreation.DnsZone;
            vars["OwnerName"] = $"{entryCreation.Subdomain}.{entryCreation.DnsZone}";
            vars["PrimaryName"] = GetPrimaryName(entryCreation.Cluster);
            man.InvokeMethod("CreateInstanceFromPropertyData", vars, null);
        }

        internal  void DeleteCname(DnsEntryDeletion deletion)
        {
            var wql = $"SELECT * FROM MicrosoftDNS_CNAMETYPE WHERE OwnerName = '{deletion.Subdomain}.{deletion.DnsZone}'";
            var query = new ObjectQuery(wql);
            var s = new ManagementObjectSearcher(_session.Value, query);
            var col = s.Get();
            foreach (ManagementObject o in col)
            {
                o.Delete();
            }
        }

        private string GetPrimaryName(string targetClusterName)
        {
            return $"rbx-{ClusterNameConvertor.GetShortName(targetClusterName)}-haproxy.lucca.local.";
        }
    }
}
