using Environments.Domain.Storage;
using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Lucca.Core.Shared.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Instances.Infra.Demos
{
    internal class SubdomainValidation
    {
        public static readonly string[] ReservedSubdomains = {
            "cc", "cc-preview", "cc-training", "certificat", "adfs", "test-adfs", "dev", "apps","app", "api", "cdn",
            "ccnet", "versioningservice", "creation-client", "dashboard", "maintenance", "partenaires", "pdfservice",
            "planificateur", "sync", "figgo", "utime", "pagga", "urba", "ilucca", "ping", "pm", "sms", "www",
            "ws-cluster", "login"
        };

        public static readonly string[] ReservedSubdomainPrefixes =
            { "ovh", "static", "adfs", "ws", "cleemy", "poplee", "timmi", "fake-", "iis-" };
    }

    public class SubdomainValidator : ISubdomainValidator
    {
        private const string SubdomainRegex = @"^(?!-)[a-z0-9-]+(?<!-)$";
        public const int SubdomainMinLength = 2;
        public const int SubdomainMaxLength = 63;
        public const int MaxDemoPerRequestSubdomain = 10;

        private readonly IDemosStore _demosStore;
        private readonly IEnvironmentsStore _environmentsStore;

        public SubdomainValidator(IDemosStore demosStore, IEnvironmentsStore environmentsStore)
        {
            _demosStore = demosStore;
            _environmentsStore = environmentsStore;
        }

        public Task ThrowIfInvalidAsync(string subdomain)
        {
            if (SubdomainValidation.ReservedSubdomains.Contains(subdomain))
            {
                throw new BadRequestException($"Subdomain {subdomain} is restricted");
            }
            if (SubdomainValidation.ReservedSubdomainPrefixes.Any(subdomain.StartsWith))
            {
                throw new BadRequestException($"Subdomain {subdomain} starts with restricted prefix");
            }
            if (subdomain.Length < SubdomainMinLength)
            {
                throw new BadRequestException($"Subdomain {subdomain} is too short (min {SubdomainMinLength} characters)");
            }
            if (subdomain.Length > SubdomainMaxLength)
            {
                throw new BadRequestException($"Subdomain {subdomain} is too long (max {SubdomainMaxLength} characters)");
            }
            if (!Regex.IsMatch(subdomain, SubdomainRegex))
            {
                throw new BadRequestException($"Subdomain {subdomain} does not match requested format (lower-case alphanumeric chars and dashes)");
            }

            return Task.CompletedTask;
        }

        public async Task<bool> IsAvailableAsync(string subdomain)
        {
            return _environmentsStore.GetAll()
                .Where(e => e.IsActive)
                .All(e => e.Subdomain.ToLower() != subdomain)
                && (await _demosStore.GetActiveAsync()).All(d => d.Subdomain.ToLower() != subdomain);
        }

        private async Task<HashSet<string>> GetUsedSubdomainsByPrefixAsync(string prefix)
        {
            var usedSubdomainsEnvs = _environmentsStore.GetAll()
                .Where(e => e.IsActive)
                .Where(e => e.Subdomain.StartsWith(prefix))
                .Select(e => e.Subdomain);

            var usedSubdomainsDemos = (await _demosStore.GetActiveAsync())
                .Where(d => d.Subdomain.StartsWith(prefix))
                .Select(e => e.Subdomain);

            var usedSubdomains = usedSubdomainsEnvs.ToList();
            usedSubdomains.AddRange(usedSubdomainsDemos.ToList());

            return usedSubdomains.ToHashSet();
        }

        public async Task<string> GetAvailableSubdomainByPrefixAsync(string prefix)
        {
            var suffixMaxLength = $"{MaxDemoPerRequestSubdomain}".Length;

            var maxSizedPrefix = Math.Min(prefix.Length, SubdomainMaxLength - suffixMaxLength);
            var maxPrefix = prefix.Substring(0, maxSizedPrefix);

            var usedSubdomains = await GetUsedSubdomainsByPrefixAsync(maxPrefix);

            for (var i = 1; i <= MaxDemoPerRequestSubdomain; i++)
            {
                var candidate = $"{maxPrefix}{i}";
                if (usedSubdomains.Contains(candidate))
                {
                    continue;
                }

                return candidate;
            }

            return null;
        }
    }
}
