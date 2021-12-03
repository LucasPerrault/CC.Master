using Lucca.Core.Rights.Abstractions;
using Lucca.Core.Rights.Abstractions.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rights.Infra.Models
{
    public class Permission : IUserPermission
    {
        public static readonly string ApiFields = $"{nameof(EstablishmentId)},{nameof(Scope)},{nameof(ExternalEntityId)},{nameof(SpecificDepartmentId)},{nameof(SpecificUserId)},{nameof(HasContextualEstablishmentAssociation)},{nameof(OperationId)}";

        public int? EstablishmentId { get; set; }
        public Scope Scope { get; set; }
        public int ExternalEntityId { get; set; }
        public int? SpecificDepartmentId { get; set; }
        public int? SpecificUserId { get; set; }
        public bool HasContextualEstablishmentAssociation { get; set; }
        public int OperationId { get; set; }
    }

    public class ApiKeyPermission : IApiKeyPermission
    {
        public static readonly string ApiFields = $"{nameof(EstablishmentId)},{nameof(Scope)},{nameof(ExternalEntityId)},{nameof(OperationId)}";

        public int? EstablishmentId { get; set; }
        public int ExternalEntityId { get; set; }
        public int OperationId { get; set; }
    }

    public static class PermissionsExtensions
    {
        private static class UserPermissionBytes
        {
            public const int TotalLength = 13;

            public static class Positions
            {
                public const int OperationId = 0;
                public const int Scope = 2;
                public const int ExternalEntityId = 4;
                public const int LegalEntityId = 6;
                public const int SpecificUserId = 8;
                public const int SpecificDepartmentId = 10;
                public const int HasContextualEstablishmentAssociation = 12;
            }
        }

        public static byte[] ToBytes(this List<IUserPermission> permissions)
        {
            var bytes = new byte[UserPermissionBytes.TotalLength * permissions.Count];
            var offset = 0;
            foreach (var permission in permissions)
            {
                BitConverter.GetBytes((ushort)permission.OperationId).CopyTo(bytes, offset + UserPermissionBytes.Positions.OperationId);
                BitConverter.GetBytes((ushort)permission.Scope).CopyTo(bytes, offset + UserPermissionBytes.Positions.Scope);
                BitConverter.GetBytes((ushort)permission.ExternalEntityId).CopyTo(bytes, offset + UserPermissionBytes.Positions.ExternalEntityId);
                BitConverter.GetBytes((ushort)(permission.EstablishmentId ?? default)).CopyTo(bytes, offset + UserPermissionBytes.Positions.LegalEntityId);
                BitConverter.GetBytes((ushort)(permission.SpecificUserId ?? default)).CopyTo(bytes, offset + UserPermissionBytes.Positions.SpecificUserId);
                BitConverter.GetBytes((ushort)(permission.SpecificDepartmentId ?? default)).CopyTo(bytes, offset + UserPermissionBytes.Positions.SpecificDepartmentId);
                BitConverter.GetBytes(permission.HasContextualEstablishmentAssociation).CopyTo(bytes, offset + UserPermissionBytes.Positions.HasContextualEstablishmentAssociation);

                offset += UserPermissionBytes.TotalLength;
            }
            return bytes;
        }

        public static byte[] ToBytes(this IUserPermission permission)
        {
            return new List<IUserPermission> { permission }.ToBytes();
        }

        public static Permission ToPermission(this byte[] bytes, int offset)
        {
            return new Permission
            {
                OperationId = BitConverter.ToUInt16(bytes, offset + UserPermissionBytes.Positions.OperationId),
                Scope = (Scope)BitConverter.ToUInt16(bytes, offset + UserPermissionBytes.Positions.Scope),
                ExternalEntityId = BitConverter.ToUInt16(bytes, offset + UserPermissionBytes.Positions.ExternalEntityId),
                EstablishmentId = ParseNonDefault(bytes, offset + UserPermissionBytes.Positions.LegalEntityId),
                SpecificUserId = ParseNonDefault(bytes, offset + UserPermissionBytes.Positions.SpecificUserId),
                SpecificDepartmentId = ParseNonDefault(bytes, offset + UserPermissionBytes.Positions.SpecificDepartmentId),
                HasContextualEstablishmentAssociation = BitConverter.ToBoolean(bytes, offset + UserPermissionBytes.Positions.HasContextualEstablishmentAssociation)
            };
        }

        public static List<Permission> ToPermissions(this byte[] bytes)
        {
            if (bytes.Length % UserPermissionBytes.TotalLength != 0)
            {
                throw new ApplicationException($"Byte array size is not a multiple of {UserPermissionBytes.TotalLength}");
            }

            return Enumerable.Range(0, bytes.Length / UserPermissionBytes.TotalLength)
                .Select(i => i * UserPermissionBytes.TotalLength)
                .Select(bytes.ToPermission)
                .ToList();
        }


        private static class ApiKeyPermissionBytes
        {
            public const int TotalLength = 6;

            public static class Positions
            {
                public const int OperationId = 0;
                public const int ExternalEntityId = 2;
                public const int LegalEntityId = 4;
            }
        }

        public static byte[] ToBytes(this List<IApiKeyPermission> permissions)
        {
            var bytes = new byte[ApiKeyPermissionBytes.TotalLength * permissions.Count];
            var offset = 0;
            foreach (var permission in permissions)
            {
                BitConverter.GetBytes((ushort)permission.OperationId).CopyTo(bytes, offset + ApiKeyPermissionBytes.Positions.OperationId);
                BitConverter.GetBytes((ushort)permission.ExternalEntityId).CopyTo(bytes, offset + ApiKeyPermissionBytes.Positions.ExternalEntityId);
                BitConverter.GetBytes((ushort)(permission.EstablishmentId ?? 0)).CopyTo(bytes, offset + ApiKeyPermissionBytes.Positions.LegalEntityId);
                offset += ApiKeyPermissionBytes.TotalLength;
            }
            return bytes;
        }

        public static byte[] ToBytes(this IApiKeyPermission permission)
        {
            return new List<IApiKeyPermission> { permission }.ToBytes();
        }

        public static IApiKeyPermission ToApiKeyPermission(this byte[] bytes, int offset)
        {
            return new ApiKeyPermission
            {
                OperationId = BitConverter.ToUInt16(bytes, offset + ApiKeyPermissionBytes.Positions.OperationId),
                ExternalEntityId = BitConverter.ToUInt16(bytes, offset + ApiKeyPermissionBytes.Positions.ExternalEntityId),
                EstablishmentId = ParseNonDefault(bytes, offset + ApiKeyPermissionBytes.Positions.LegalEntityId),
            };
        }

        public static List<IApiKeyPermission> ToApiKeyPermissions(this byte[] bytes)
        {
            if (bytes.Length % ApiKeyPermissionBytes.TotalLength != 0)
            {
                throw new ApplicationException($"Byte array size is not a multiple of {ApiKeyPermissionBytes.TotalLength}");
            }

            return Enumerable.Range(0, bytes.Length / ApiKeyPermissionBytes.TotalLength)
                .Select(i => i * ApiKeyPermissionBytes.TotalLength)
                .Select(bytes.ToApiKeyPermission)
                .ToList();
        }

        private static ushort? ParseNonDefault(byte[] bytes, int index)
        {
            var parsedValue = BitConverter.ToUInt16(bytes, index);
            return parsedValue == default
                ? (ushort?)null
                : parsedValue;
        }
    }
}
