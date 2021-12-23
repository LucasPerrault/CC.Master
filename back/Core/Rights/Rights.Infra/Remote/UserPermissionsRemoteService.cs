using Lucca.Core.Rights.Abstractions;
using Lucca.Core.Rights.Abstractions.Permissions;
using Remote.Infra.Services;
using Rights.Infra.Models;
using Rights.Infra.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rights.Infra.Remote
{
    public class UserPermissionsRemoteService
    {
        private readonly RestApiV3HttpClientHelper _httpClientHelper;

        public UserPermissionsRemoteService(HttpClient httpClient)
        {
            _httpClientHelper = new RestApiV3HttpClientHelper(httpClient, "Partenaires users permissions");
        }

        internal async Task<IReadOnlyCollection<IUserPermission>> GetUserPermissionsAsync(int principalId)
        {
            var allPermissions = await GetAllUserPermissionsAsync(principalId);
            return allPermissions.ToList();
        }

        private async Task<IEnumerable<Permission>> GetAllUserPermissionsAsync(int principalId)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "appInstanceId", RightsHelper.CloudControlAppInstanceId.ToString() },
                { "userId", principalId.ToString() },
                { "fields", PermissionDto.ApiFields }
            };

            var userPermissions = await _httpClientHelper.GetObjectCollectionResponseAsync<PermissionDto>(queryParams);
            var dtos = userPermissions.Data.Items;
            return dtos.Select(dto => dto.ToPermission());
        }

        internal class PermissionDto
        {
            public static readonly string ApiFields = $"{nameof(LegalEntityId)},{nameof(Scope)},{nameof(ExternalEntityId)},{nameof(SpecificDepartmentId)},{nameof(SpecificUserId)},{nameof(HasContextualLegalEntityAssociation)},{nameof(OperationId)}";

            public int? LegalEntityId { get; set; }
            public Scope Scope { get; set; }
            public int ExternalEntityId { get; set; }
            public int? SpecificDepartmentId { get; set; }
            public int? SpecificUserId { get; set; }
            public bool HasContextualLegalEntityAssociation { get; set; }
            public int OperationId { get; set; }

            public Permission ToPermission() => new Permission
            {
                Scope = Scope,
                EstablishmentId = LegalEntityId,
                OperationId = OperationId,
                ExternalEntityId = ExternalEntityId,
                SpecificDepartmentId = SpecificDepartmentId,
                SpecificUserId = SpecificUserId,
                HasContextualEstablishmentAssociation = HasContextualLegalEntityAssociation
            };
        }
    }
}
