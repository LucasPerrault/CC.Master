﻿using Rights.Domain.Filtering;
using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Domain.Filtering;

namespace Users.Domain
{
    public interface IUsersStore
    {
        Task<SimpleUser> GetByIdAsync(int id, AccessRight accessRight);
        Task<bool> ExistsByIdAsync(int userId, AccessRight accessRight);
        Task<List<SimpleUser>> GetAllAsync(UsersFilter filter, AccessRight accessRight);
    }
}
