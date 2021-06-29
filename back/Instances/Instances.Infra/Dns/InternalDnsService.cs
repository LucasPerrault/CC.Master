using Instances.Infra.Shared;
using System;
using System.Management;

namespace Instances.Infra.Dns
{
    public class InternalDnsConfiguration
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Authority { get; set; }
        public string Server { get; set; }
    }

    public class InternalDnsService
    {
        private readonly Lazy<ManagementScope> _session;
        private readonly string _server;

        public InternalDnsService(InternalDnsConfiguration configuration)
        {
            var con = new ConnectionOptions
            {
                Username = configuration.Username,
                Password = configuration.Password,
                Authority = configuration.Authority
            };
            _server = configuration.Server;

            _session = new Lazy<ManagementScope>
            (
                () =>
                {
                    var session = new ManagementScope(@$"\\{_server}\root\microsoftdns")
                    {
                        Options = con
                    };
                    session.Connect();
                    return session;
                }
            );
        }

        public void AddNewCname(string domain, string targetClusterName)
        {
            var man = new ManagementClass(_session.Value, new ManagementPath("MicrosoftDNS_CNAMETYPE"), null);
            var vars = man.GetMethodParameters("CreateInstanceFromPropertyData");
            vars["DnsServerName"] = _server;
            vars["ContainerName"] = domain.Split(".", 2)[1];
            vars["OwnerName"] = domain;
            vars["PrimaryName"] = GetPrimaryName(targetClusterName);
            man.InvokeMethod("CreateInstanceFromPropertyData", vars, null);
        }

        private string GetPrimaryName(string targetClusterName)
        {
            return $"rbx-{ClusterNameConvertor.GetShortName(targetClusterName)}-haproxy.lucca.local.";
        }

        internal void DeleteCname(string domain)
        {
            var wql = $"SELECT * FROM MicrosoftDNS_CNAMETYPE WHERE OwnerName = '{domain}'";
            var query = new ObjectQuery(wql);
            var s = new ManagementObjectSearcher(_session.Value, query);
            var col = s.Get();
            foreach (ManagementObject o in col)
            {
                o.Delete();
            }
        }
    }
}
