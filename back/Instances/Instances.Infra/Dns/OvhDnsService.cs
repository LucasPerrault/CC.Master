using Instances.Domain.Instances;
using Instances.Infra.Shared;
using Microsoft.Extensions.Logging;
using Ovh.Api;
using System.Threading.Tasks;
using Tools;

namespace Instances.Infra.Dns
{
    public class OvhDnsConfiguration
    {
        public string Endpoint { get; set; }
        public string ApplicationKey { get; set; }
        public string ApplicationSecret { get; set; }
        public string ConsumerKey { get; set; }
    }

    public class OvhDnsService : IExternalDnsService
    {
        // https://api.ovh.com/console/#/domain/zone/%7BzoneName%7D/record#GET
        private const string GetRecordApiPath = "/domain/zone/{zoneName}/record?fieldType=CNAME&subDomain={subDomain}";
        // https://api.ovh.com/console/#/domain/zone/%7BzoneName%7D/record#POST
        private const string CreateRecordApiPath = "/domain/zone/{zoneName}/record";
        // https://api.ovh.com/console/#/domain/zone/%7BzoneName%7D/record/%7Bid%7D#DELETE
        private const string DeleteRecordApiPath = "/domain/zone/{zoneName}/record/{id}";
        // https://api.ovh.com/console/#/domain/zone/%7BzoneName%7D/refresh#POST
        private const string RefreshZoneApiPath = "/domain/zone/{zoneName}/refresh";

        private readonly Client _ovhClient;
        private readonly ILogger<OvhDnsService> _logger;

        public OvhDnsService(Client ovhClient, ILogger<OvhDnsService> logger)
        {
            _ovhClient = ovhClient;
            _logger = logger;
        }

        public async Task AddNewCnameAsync(DnsEntryCreation entryCreation)
        {
            var payload = new CNameCreationDto
            {
                SubDomain = entryCreation.Subdomain,
                Target = GetCNameTargetName(entryCreation),
            };
            _logger.LogDebug("Create new CNAME on ovh : {from} ({zone}) to {target} ", payload.SubDomain, entryCreation.DnsZone, payload.Target);

            await _ovhClient.PostStringAsync(GetOvhApiPath(CreateRecordApiPath, entryCreation), Serializer.Serialize(payload));
            await _ovhClient.PostAsync(GetOvhApiPath(RefreshZoneApiPath, entryCreation));
        }

        public async Task DeleteCnameAsync(DnsEntryDeletion entryDeletion)
        {
            var ovhRecordId = await _ovhClient.GetAsync<long[]>(GetOvhApiPath(GetRecordApiPath, entryDeletion));
            if (ovhRecordId.Length == 0) { return; }

            _logger.LogDebug("Delete CNAME on ovh : {domain} ({zone})", entryDeletion.Subdomain, entryDeletion.DnsZone);
            await _ovhClient.DeleteAsync(GetOvhApiPathWithId(DeleteRecordApiPath, entryDeletion, ovhRecordId[0]));
            await _ovhClient.PostAsync(GetOvhApiPath(RefreshZoneApiPath, entryDeletion));
        }

        private string GetOvhApiPath(string apiPathSeed,  IDnsEntry dnsEntry)
        {
            return apiPathSeed.Replace("{zoneName}", dnsEntry.DnsZone).Replace("{subDomain}", dnsEntry.Subdomain);
        }

        private string GetOvhApiPathWithId(string apiPathSeed, IDnsEntry dnsEntry, long id)
        {
            return GetOvhApiPath(apiPathSeed, dnsEntry).Replace("{id}", $"{id}");
        }

        private string GetCNameTargetName(DnsEntryCreation entryCreation)
        {
            if (entryCreation.Cluster == IDnsService.RedirectionCluster)
            {
                return "redirection.ilucca.net.";
            }
            return $"ovh-{ClusterNameConvertor.GetLongName(entryCreation.Cluster)}.{entryCreation.DnsZone}.";
        }

        private class CNameCreationDto
        {
            public string FieldType => "CNAME";
            public string SubDomain { get; set; }
            public string Target { get; set; }
            public long? Ttl => null;
        }
    }
}
