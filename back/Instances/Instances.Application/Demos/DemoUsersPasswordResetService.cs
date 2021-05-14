using Instances.Application.Instances;
using Instances.Domain.Demos;
using Lucca.Core.Shared.Domain.Exceptions;
using System.Threading.Tasks;

namespace Instances.Application.Demos
{
    public interface IDemoUsersPasswordResetService
    {
        Task ResetPasswordAsync(Demo demo, string password);
    }

    public class DemoUsersPasswordResetService : IDemoUsersPasswordResetService
    {
        private readonly IUsersPasswordResetService _resetService;

        public DemoUsersPasswordResetService(IUsersPasswordResetService resetService)
        {
            _resetService = resetService;
        }

        public Task ResetPasswordAsync(Demo demo, string password)
        {
            if (demo.IsTemplate)
            {
                throw new BadRequestException("Cannot reset passwords for template demos");
            }

            return _resetService.ResetPasswordAsync(demo.Href, password);
        }
    }
}
