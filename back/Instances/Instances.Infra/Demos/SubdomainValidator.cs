using Environments.Domain.Storage;
using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Lucca.Core.Shared.Domain.Exceptions;
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
        public const int SubdomainMaxLength = 200;
        private const int MaxDemoPerRequestSubdomain = 10;

        private readonly IDemosStore _demosStore;
        private readonly IEnvironmentsStore _environmentsStore;

        public SubdomainValidator(IDemosStore demosStore, IEnvironmentsStore environmentsStore)
        {
            _demosStore = demosStore;
            _environmentsStore = environmentsStore;
        }

        public async Task<string> GetSubdomainAsync(string candidate, bool useAsPrefix)
        {
            await ThrowIfInvalidAsync(candidate);

            if (IsAvailable(candidate))
            {
                return candidate;
            }

            if (!useAsPrefix)
            {
                throw new BadRequestException($"Subdomain {candidate} is not available");
            }

            var availableSubdomain = GetAvailableSubdomainByPrefix(candidate);
            if (string.IsNullOrEmpty(availableSubdomain))
            {
                throw new BadRequestException($"Subdomain {candidate} is not available (limit reached for this prefix)");
            }

            return availableSubdomain;
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

        public bool IsAvailable(string subdomain)
        {
            return _environmentsStore.GetAll()
                .Where(e => e.IsActive)
                .All(e => e.Subdomain.ToLower() != subdomain)
                && _demosStore.GetActive().All(d => d.Subdomain.ToLower() != subdomain);
        }

        private HashSet<string> GetUsedSubdomainsByPrefix(string prefix)
        {
            var usedSubdomainsEnvs = _environmentsStore.GetAll()
                .Where(e => e.IsActive)
                .Where(e => e.Subdomain.StartsWith(prefix))
                .Select(e => e.Subdomain);

            var usedSubdomainsDemos = _demosStore.GetActive()
                .Where(d => d.Subdomain.StartsWith(prefix))
                .Select(e => e.Subdomain);

            var usedSubdomains = usedSubdomainsEnvs.ToList();
            usedSubdomains.AddRange(usedSubdomainsDemos.ToList());

            return usedSubdomains.ToHashSet();
        }

        public string GetAvailableSubdomainByPrefix(string prefix)
        {
            var usedSubdomains = GetUsedSubdomainsByPrefix(prefix);

            for (var i = 1; i <= MaxDemoPerRequestSubdomain; i++)
            {
                var candidate = $"{prefix}{i}";
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
