using System;
using System.Threading.Tasks;

namespace Instances.Application.Instances
{
    public interface IUsersPasswordResetService
    {
        Task ResetPasswordAsync(Uri uri, string password);
    }
}
