using Environments.Domain.Storage;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Demos.Validation;
using Instances.Domain.Instances;
using Lucca.Core.Shared.Domain.Exceptions;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;

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
        public const int MaxDemoPerRequestSubdomain = 10;

        private readonly IDemosStore _demosStore;
        private readonly IEnvironmentsStore _environmentsStore;
        private readonly IInstanceDuplicationsStore _duplicationsStore;
        private readonly ISubdomainValidationTranslator _translator;

        public SubdomainValidator
        (
            IDemosStore demosStore,
            IEnvironmentsStore environmentsStore,
            IInstanceDuplicationsStore duplicationsStore,
            ISubdomainValidationTranslator translator
        )
        {
            _demosStore = demosStore;
            _environmentsStore = environmentsStore;
            _duplicationsStore = duplicationsStore;
            _translator = translator;
        }

        public Task ThrowIfInvalidAsync(string subdomain)
        {
            var validity = SubdomainFormatChecker.GetValidity(subdomain);

            if (validity != SubdomainValidity.Ok)
            {
                var message = _translator.GetInvalidityMessage(subdomain, validity);
                throw new BadRequestException(message);
            }

            return Task.CompletedTask;
        }

        public async Task ThrowIfUnavailableAsync(string subdomain)
        {
            var available = await IsAvailableAsync(subdomain);
            if (!available)
            {
                var message = _translator.GetUnavailabilityMessage(subdomain);
                throw new BadRequestException(message);
            }
        }

        public async Task<bool> IsAvailableAsync(string subdomain)
        {
            var envs = await _environmentsStore.GetAsync
            (
                EnvironmentAccessRight.Everything,
                new EnvironmentFilter
                {
                    IsActive = CompareBoolean.TrueOnly,
                    Subdomain = CompareString.Equals(subdomain),
                }
            );

            var demos = await _demosStore.GetAsync(new DemoFilter
            {
                IsActive = CompareBoolean.TrueOnly,
                Subdomain = CompareString.Equals(subdomain),
            }, AccessRight.All);

            var duplications = await _duplicationsStore.GetPendingForSubdomainAsync(subdomain);

            return !envs.Any()
                && !demos.Any()
                && !duplications.Any();
        }

        private async Task<HashSet<string>> GetUsedSubdomainsByPrefixAsync(string prefix)
        {
            var envsWithSubdomain = await _environmentsStore
                .GetAsync
                (
                    EnvironmentAccessRight.Everything,
                    new EnvironmentFilter
                    {
                        IsActive = CompareBoolean.TrueOnly,
                        Subdomain = CompareString.StartsWith(prefix)
                    }
                );

            var usedSubdomainsEnvs = envsWithSubdomain.Select(e => e.Subdomain);

            var filter = new DemoFilter
            {
                IsActive = CompareBoolean.TrueOnly,
                Search = prefix,
                Subdomain = CompareString.StartsWith(prefix)
            };
            var usedSubdomainsDemos = (await _demosStore.GetAsync(filter, AccessRight.All))
                .Select(e => e.Subdomain);

            var usedSubdomains = usedSubdomainsEnvs.ToList();
            usedSubdomains.AddRange(usedSubdomainsDemos.ToList());

            return usedSubdomains.ToHashSet();
        }

        public async Task<string> GetAvailableSubdomainByPrefixAsync(string prefix)
        {
            var suffixMaxLength = $"{MaxDemoPerRequestSubdomain}".Length;

            var maxSizedPrefix = Math.Min(prefix.Length, SubdomainExtensions.SubdomainMaxLength - suffixMaxLength);
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
