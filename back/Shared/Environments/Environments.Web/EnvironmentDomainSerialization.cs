using Environments.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace Environments.Web
{
    public static class EnvironmentDomainSerialization
    {
        private static readonly Lazy<IReadOnlyDictionary<string, EnvironmentDomain>> _domainsPerDescription = new Lazy<IReadOnlyDictionary<string, EnvironmentDomain>>(GetDomainsPerDescription);

        private static readonly Lazy<IReadOnlyDictionary<EnvironmentDomain, string>> _descriptionsPerDomain = new Lazy<IReadOnlyDictionary<EnvironmentDomain, string>>(GetDescriptionsPerDomain);

        public static string ToDescription(this EnvironmentDomain environmentDomain)
        {
            return _descriptionsPerDomain.Value[environmentDomain];
        }

        public static EnvironmentDomain? ToEnvironmentDomain(this string description)
        {
            if (_domainsPerDescription.Value.TryGetValue(description, out var value))
            {
                return value;
            }

            return null;
        }

        private static IReadOnlyDictionary<string, EnvironmentDomain> GetDomainsPerDescription()
        {
            var enums =  Enum.GetValues(typeof(EnvironmentDomain)).Cast<EnvironmentDomain>();
            var dictionary =  new Dictionary<string, EnvironmentDomain>(StringComparer.OrdinalIgnoreCase);

            foreach (var enumValue in enums)
            {
                dictionary[enumValue.GetDescription()] = enumValue;
            }

            return dictionary;
        }

        private static IReadOnlyDictionary<EnvironmentDomain, string> GetDescriptionsPerDomain()
        {
            return Enum.GetValues(typeof(EnvironmentDomain))
                .Cast<EnvironmentDomain>()
                .ToDictionary(e => e, e => e.GetDescription());
        }
    }
}
