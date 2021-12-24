using Environments.Application;
using Environments.Domain;
using Instances.Web.Dtos;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;

namespace Environments.Web.Controllers
{
    [ApiController]
    [Route("/api/environment-renaming")]
    public class EnvironmentRenamingController
    {
        private readonly IEnvironmentRenamingService _environmentRenamingService;

        public EnvironmentRenamingController(IEnvironmentRenamingService environmentRenamingService)
        {
            _environmentRenamingService = environmentRenamingService;
        }

        [HttpPost]
        [ForbidIfMissing(Operation.ReadCodeSources)] // TODO A changer !
        public async Task<EnvironmentRenamingStatusDetail> RenameEnvironmentAsync([FromBody] EnvironmentRenamingDto environmentRenaming)
        {
            return await _environmentRenamingService.RenameAsync(environmentRenaming.EnvironmentId, environmentRenaming.NewName);
        }
    }
}
