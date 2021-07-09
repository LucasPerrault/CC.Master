using Instances.Infra.Shared;
using Newtonsoft.Json;
using Ovh.Api;
using System;
using System.Threading.Tasks;

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

        public OvhDnsService(Client ovhClient)
        {
            _ovhClient = ovhClient;
        }

        public async Task AddNewCnameAsync(DnsEntryCreation entryCreation)
        {
            var payload = new CNameCreationDto
            {
                SubDomain = entryCreation.Subdomain,
                Target = GetCNameTargetName(entryCreation),
            };

            await _ovhClient.PostAsync(GetOvhApiPath(CreateRecordApiPath, entryCreation), payload);
            await _ovhClient.PostAsync(GetOvhApiPath(RefreshZoneApiPath, entryCreation));
        }

        public async Task DeleteCnameAsync(DnsEntryDeletion entryDeletion)
        {
            var ovhRecordId = await _ovhClient.GetAsync<long[]>(GetOvhApiPath(GetRecordApiPath, entryDeletion));
            if (ovhRecordId.Length == 0) { return; }

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
            return $"ovh-{ClusterNameConvertor.GetLongName(entryCreation.Cluster)}.{entryCreation.DnsZone}.";
        }

        private class CNameCreationDto
        {
            [JsonProperty("fieldType")]
            public string FieldType => "CNAME";
            [JsonProperty("subDomain")]
            public string SubDomain { get; set; }
            [JsonProperty("target")]
            public string Target { get; set; }
            [JsonProperty("ttl", NullValueHandling = NullValueHandling.Ignore)]
            public long? Ttl => null;
        }
    }
}
