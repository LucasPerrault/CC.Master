using System;
using System.Threading.Tasks;

namespace Users.Domain
{
    public interface IUsersService
    {
        Task<User> GetByTokenAsync(Guid token);
    }

    public class UnknownDepartmentCodeException : ApplicationException
    {
        public UnknownDepartmentCodeException(string departmentCode) : base($"Unknown user department code {departmentCode}")
        { }
    }
}
