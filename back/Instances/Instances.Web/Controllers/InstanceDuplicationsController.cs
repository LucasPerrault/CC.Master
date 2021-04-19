using Instances.Application.Demos;
using Instances.Domain.Instances;
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
        public async Task<InstanceDuplication> GetDuplicationAsync([FromRoute]Guid id)
        {
            return await _duplicationsRepository.GetDuplication(id);
        }
    }
}
