using Authentication.Domain;
using Lucca.Core.Shared.Domain.Contracts.Principals;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Infra.Stores;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rights.Web.Controllers
{
    [Route("api/principals")]
    public class PrincipalsController
    {
        private readonly ClaimsPrincipal _principal;
        private readonly ICloudControlPermissionsStore _permissionsStore;

        public PrincipalsController(ClaimsPrincipal principal, ICloudControlPermissionsStore permissionsStore)
        {
            _principal = principal;
            _permissionsStore = permissionsStore;
        }

        [HttpGet("me")]
        public async Task<MeDto> WhoAmIAsync()
        {
            return _principal switch
            {
                CloudControlUserClaimsPrincipal user => new MeDto
                {
                    PrincipalType = PrincipalType.User,
                    Id = user.UserId.Value,
                    Mail = user.User.Mail,
                    Name = user.Name,
                    Token = user.Token,
                    Culture = new CultureDto { Code = CultureInfo.CurrentCulture.Name },
                    Permissions = await GetPermissionsAsync(user),
                    DistributorId = user.User.DistributorId
                },
                CloudControlApiKeyClaimsPrincipal apiKey => new MeDto
                {
                    PrincipalType = PrincipalType.ApiKey,
                    Id = apiKey.ApiKey.Id,
                    Mail = string.Empty,
                    Name = apiKey.ApiKey.Name,
                    Token = apiKey.Token,
                    Permissions = await GetPermissionsAsync(apiKey)
                },
                _ => throw new ApplicationException("Unhandled principal type")
            };
        }

        private async Task<List<PermissionDto>> GetPermissionsAsync(CloudControlUserClaimsPrincipal user)
        {
            var permissions = await _permissionsStore.GetUserPermissionsAsync(user.User.Id);

            return permissions.Select
            (
                p => new PermissionDto
                {
                    Operation = new OperationDto { Id = p.OperationId }
                }
            )
                .ToList();
        }

        private async Task<List<PermissionDto>> GetPermissionsAsync(CloudControlApiKeyClaimsPrincipal apiKey)
        {
            var operations = OperationHelper.GetAll().Select(o => (int)o).ToHashSet();
            var permissions = await _permissionsStore.GetApiKeyPermissionsAsync(apiKey.ApiKey.Id, operations);

            return permissions.Select
            (
                p => new PermissionDto
                {
                    Operation = new OperationDto { Id = p.OperationId }
                }
            )
                .ToList();
        }
    }

    public class MeDto
    {
        public PrincipalType PrincipalType { get; set; }
        public int Id { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }
        public Guid Token { get; set; }
        public CultureDto Culture { get; set; }
        public int DistributorId { get; set; }
        public List<PermissionDto> Permissions { get; set; }
    }

    public class PermissionDto
    {
        public OperationDto Operation { get; set; }
    }

    public class OperationDto
    {
        public int Id { get; set; }
    }

    public class CultureDto
    {
        public string Code { get; set; }
    }
}
