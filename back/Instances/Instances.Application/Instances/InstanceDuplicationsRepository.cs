using Instances.Application.Instances.Dtos;
using Instances.Domain.Instances;
using Lucca.Core.Shared.Domain.Exceptions;
using Resources.Translations;
using System;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public class InstanceDuplicationsRepository
    {
        private readonly IInstanceDuplicationsStore _duplicationsStore;
        private readonly ISubdomainValidator _subdomainValidator;
        private readonly IInstancesTranslations _translations;

        public InstanceDuplicationsRepository(IInstanceDuplicationsStore duplicationsStore, ISubdomainValidator subdomainValidator, IInstancesTranslations translations)
        {
            _duplicationsStore = duplicationsStore;
            _subdomainValidator = subdomainValidator;
            _translations = translations;
        }

        public Task<InstanceDuplication> GetDuplication(Guid id)
        {
            return _duplicationsStore.GetAsync(id);
        }

        public async Task<SubdomainUsability> GetUsabilityAsync(string subdomain)
        {
            try
            {
                await _subdomainValidator.ThrowIfInvalidAsync(subdomain);
                await _subdomainValidator.ThrowIfUnavailableAsync(subdomain);
            }
            catch (BadRequestException e)
            {
                return new SubdomainUsability
                {
                    IsUsable = false,
                    Message = e.Message
                };
            }

            return new SubdomainUsability
            {
                IsUsable = true,
                Message = _translations.SubdomainOk()
            };
        }
    }
}
