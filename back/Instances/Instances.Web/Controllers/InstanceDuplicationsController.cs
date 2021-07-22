using Instances.Application.Instances;
using Instances.Application.Instances.Dtos;
using Instances.Domain.Instances;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Instances.Web.Controllers
{
    [ApiController, Route("/api/instanceduplications")]
    public class InstanceDuplicationsController
    {
        private readonly InstanceDuplicationsRepository _duplicationsRepository;

        public InstanceDuplicationsController(InstanceDuplicationsRepository duplicationsRepository)
        {
            _duplicationsRepository = duplicationsRepository;
        }

        [HttpGet("{id}")]
        public Task<InstanceDuplication> GetDuplicationAsync([FromRoute]Guid id)
        {
            return _duplicationsRepository.GetDuplication(id);
        }

        [HttpGet("usability")]
        public Task<SubdomainUsability> GetUsabilityAsync([FromQuery]string subdomain)
        {
            if (string.IsNullOrEmpty(subdomain))
            {
                throw new BadRequestException($"{nameof(subdomain)} parameter is mandatory");
            }

            return _duplicationsRepository.GetUsabilityAsync(subdomain);
        }
    }
}
