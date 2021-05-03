using Instances.Domain.Demos;
using Microsoft.AspNetCore.WebUtilities;
using Remote.Infra.Extensions;
using Remote.Infra.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instances.Infra.Demos
{
    public class HubspotConfiguration
    {
        public Uri ServerUri { get; set; }
        public Guid OutboundToken { get; set; }
    }

    public class HubspotService : IHubspotService
    {
        private const string _authQueryParam = "hapikey";

        private readonly HttpClient _client;
        private readonly HubspotConfiguration _configuration;
        private readonly HttpResponseMessageParser _parser;

        public HubspotService(HttpClient client, HubspotConfiguration configuration, HttpResponseMessageParser parser)
        {
            _client = client;
            _configuration = configuration;
            _parser = parser;
        }

        public async Task CallWorkflowForEmailAsync(int workflowId, string email)
        {
            var path = $"automation/v2/workflows/{workflowId}/enrollments/contacts/{email}";
            await _client.PostAsync(AuthenticateWithQueryParams(path), null);
        }

        public async Task<HubspotContact> GetContactAsync(int vId)
        {
            var path = GetContactUrl(vId);
            var responseMessage = await _client.GetAsync(AuthenticateWithQueryParams(path));

            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new ApplicationException($"Hubspot contact fetch failed with status {responseMessage.StatusCode}");
            }

            var dto = await _parser.ParseAsync<HubspotContactDto>(responseMessage);
            return new HubspotContact
            {
                VId = dto.VId,
                Email = dto.Properties.Email.Value,
                Company = dto.Properties.Company.Value,
                IpAddress = dto.Properties.IpAddress.Value
            };
        }

        public async Task UpdateContactSubdomainAsync(int vId, string subdomain)
        {
            var hubspotPropertySetter = new { properties = new[] { new { property = "demo_url", value = subdomain } } };
            var path = GetContactUrl(vId);
            await _client.PostAsync(AuthenticateWithQueryParams(path), hubspotPropertySetter.ToJsonPayload());
        }

        private static string GetContactUrl(int vId)
        {
            return $"contacts/v1/contact/vid/{vId}/profile";
        }

        private string AuthenticateWithQueryParams(string path)
        {
            var authQueryParams = new Dictionary<string, string>
            {
                [_authQueryParam] = _configuration.OutboundToken.ToString()
            };

            return QueryHelpers.AddQueryString(path, authQueryParams);
        }

        class HubspotContactDto
        {

            public int VId { get; set; }
            public PropertiesGetter Properties { get; set; }

            public class PropertiesGetter
            {
                public APIField Email { get; set; }
                public APIField Company { get; set; }
                public APIField IpAddress { get; set; }
            }

            public class APIField
            {
                public string Value { get; set; }
            }
        }
    }
}
