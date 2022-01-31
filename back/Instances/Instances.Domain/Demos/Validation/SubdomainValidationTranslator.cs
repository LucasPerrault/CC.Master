using Instances.Domain.Instances;
using Resources.Translations;
using System;

namespace Instances.Domain.Demos.Validation
{
    public interface ISubdomainValidationTranslator
    {
        string GetInvalidityMessage(string subdomain, SubdomainValidity validity);
        string GetUnavailabilityMessage(string subdomain);
    }

    public class SubdomainValidationTranslator : ISubdomainValidationTranslator
    {
        private readonly IInstancesTranslations _translations;

        public SubdomainValidationTranslator(IInstancesTranslations translations)
        {
            _translations = translations;
        }

        public string GetInvalidityMessage(string subdomain, SubdomainValidity validity)
        {
            return validity switch
            {
                SubdomainValidity.ReservedWord => _translations.SubdomainReservedWord(subdomain),
                SubdomainValidity.ReservedPrefix => _translations.SubdomainStartsWithReservedWord(subdomain),
                SubdomainValidity.TooShort => _translations.SubdomainTooShort(subdomain, SubdomainExtensions.SubdomainMinLength),
                SubdomainValidity.TooLong => _translations.SubdomainTooLong(subdomain, SubdomainExtensions.SubdomainMaxLength),
                SubdomainValidity.WrongFormat => _translations.SubdomainInvalid(subdomain),
                _ => throw new NotSupportedException()
            };
        }

        public string GetUnavailabilityMessage(string subdomain)
        {
            return _translations.SubdomainUnavailable(subdomain);
        }
    }
}
