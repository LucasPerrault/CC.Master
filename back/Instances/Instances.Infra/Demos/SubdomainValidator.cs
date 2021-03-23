﻿using Environments.Infra.Storage.Stores;
using Instances.Domain.Instances;
using Instances.Infra.Storage.Stores;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
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
        private const string SubdomainRegex = @"^(?!-)[a-z0-9-]{1,63}(?<!-)$";
        private const int SubdomainMinLength = 2;
        private const int SubdomainMaxLength = 200;
        private const int MaxDemoPerRequestSubdomain = 10;

        private readonly DemosStore _demosStore;
        private readonly EnvironmentsStore _environmentsStore;

        public SubdomainValidator(DemosStore demosStore, EnvironmentsStore environmentsStore)
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
            var envTask = _environmentsStore.GetAllAsync()
                .Where(e => e.IsActive)
                .AllAsync(e => e.Subdomain != subdomain);

            var demoTask = _demosStore.GetAllAsync()
                .Where(d => d.IsActive)
                .AllAsync(d => d.Subdomain != subdomain);

            return await envTask && await demoTask;
        }

        private HashSet<string> GetUsedSubdomainsByPrefix(string prefix)
        {
            var usedSubdomainsEnvs = _environmentsStore.GetAllAsync()
                .Where(e => e.IsActive)
                .Where(e => e.Subdomain.StartsWith(prefix))
                .Select(e => e.Subdomain);

            var usedSubdomainsDemos = _demosStore.GetAllAsync()
                .Where(d => d.IsActive)
                .Where(d => d.Subdomain.StartsWith(prefix))
                .Select(e => e.Subdomain);

            var usedSubdomains = usedSubdomainsEnvs.ToList();
            usedSubdomains.AddRange(usedSubdomainsDemos.ToList());

            return usedSubdomains.ToHashSet();
        }

        public async Task<string> GetAvailableSubdomainAsync(string subdomain)
        {
            var usedSubdomains = GetUsedSubdomainsByPrefix(subdomain);
            if (await IsAvailableAsync(subdomain))
            {
                return subdomain;
            }

            for (var i = 1; i <= MaxDemoPerRequestSubdomain; i++)
            {
                var candidate = $"{subdomain}{i}";
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
