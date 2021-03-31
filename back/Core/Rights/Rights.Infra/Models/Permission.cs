using Lucca.Core.Rights.Abstractions;
using Lucca.Core.Rights.Abstractions.Permissions;

namespace Rights.Infra.Models
{
    internal class Permission : IUserPermission
    {
        public static readonly string ApiFields = $"{nameof(LegalEntityId)},{nameof(Scope)},{nameof(ExternalEntityId)},{nameof(SpecificDepartmentId)},{nameof(SpecificUserId)},{nameof(HasContextualLegalEntityAssociation)},{nameof(OperationId)}";

        public int? LegalEntityId { get; set; }
        public Scope Scope { get; set; }
        public int ExternalEntityId { get; set; }
        public int? SpecificDepartmentId { get; set; }
        public int? SpecificUserId { get; set; }
        public bool HasContextualLegalEntityAssociation { get; set; }
        public int OperationId { get; set; }
    }

    internal class ApiKeyPermission : IApiKeyPermission
    {
        public static readonly string ApiFields = $"{nameof(LegalEntityId)},{nameof(Scope)},{nameof(ExternalEntityId)},{nameof(OperationId)}";

        public int? LegalEntityId { get; set; }
        public Scope Scope { get; set; }
        public int ExternalEntityId { get; set; }
        public int OperationId { get; set; }
    }
}
