using Lucca.Core.Rights.Abstractions;
using Lucca.Core.Rights.Abstractions.Permissions;
using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rights.Infra.Models
{
    public class Permission : IUserPermission
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

    public class ApiKeyPermission : IApiKeyPermission
    {
        public static readonly string ApiFields = $"{nameof(LegalEntityId)},{nameof(Scope)},{nameof(ExternalEntityId)},{nameof(OperationId)}";

        public int? LegalEntityId { get; set; }
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
                public const int HasContextualLegalEntityAssociation = 12;
            }
        }

        public static byte[] ToBytes(this List<IUserPermission> permissions)
        {
            var bytes = new byte[UserPermissionBytes.TotalLength * permissions.Count];
            var offset = 0;
            foreach (var permissionBytes in permissions.Select(p => p.ToBytes()))
            {
                Buffer.BlockCopy(permissionBytes, 0, bytes, offset, permissionBytes.Length);
                offset += permissionBytes.Length;
            }
            return permissions.SelectMany(p => p.ToBytes()).ToArray();
        }

        public static byte[] ToBytes(this IUserPermission permission)
        {
            var bytes = new byte[UserPermissionBytes.TotalLength];
            BitConverter.GetBytes((ushort)permission.OperationId).CopyTo(bytes, UserPermissionBytes.Positions.OperationId);
            BitConverter.GetBytes((ushort)permission.Scope).CopyTo(bytes, UserPermissionBytes.Positions.Scope);
            BitConverter.GetBytes((ushort)permission.ExternalEntityId).CopyTo(bytes, UserPermissionBytes.Positions.ExternalEntityId);
            BitConverter.GetBytes((ushort)(permission.LegalEntityId ?? default)).CopyTo(bytes, UserPermissionBytes.Positions.LegalEntityId);
            BitConverter.GetBytes((ushort)(permission.SpecificUserId ?? default)).CopyTo(bytes, UserPermissionBytes.Positions.SpecificUserId);
            BitConverter.GetBytes((ushort)(permission.SpecificDepartmentId ?? default)).CopyTo(bytes, UserPermissionBytes.Positions.SpecificDepartmentId);
            BitConverter.GetBytes(permission.HasContextualLegalEntityAssociation).CopyTo(bytes, UserPermissionBytes.Positions.HasContextualLegalEntityAssociation);
            return bytes;
        }

        public static Permission ToPermission(this byte[] bytes)
        {
            return new Permission
            {
                OperationId = BitConverter.ToUInt16(bytes, UserPermissionBytes.Positions.OperationId),
                Scope = (Scope)BitConverter.ToUInt16(bytes, UserPermissionBytes.Positions.Scope),
                ExternalEntityId = BitConverter.ToUInt16(bytes, UserPermissionBytes.Positions.ExternalEntityId),
                LegalEntityId = ParseNonDefault(bytes, UserPermissionBytes.Positions.LegalEntityId),
                SpecificUserId = ParseNonDefault(bytes, UserPermissionBytes.Positions.SpecificUserId),
                SpecificDepartmentId = ParseNonDefault(bytes, UserPermissionBytes.Positions.SpecificDepartmentId),
                HasContextualLegalEntityAssociation = BitConverter.ToBoolean(bytes, UserPermissionBytes.Positions.HasContextualLegalEntityAssociation)
            };
        }

        public static List<Permission> ToPermissions(this byte[] bytes)
        {
            return bytes.ToBatches(UserPermissionBytes.TotalLength).Select(b => b.ToPermission()).ToList();
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
            foreach (var permissionBytes in permissions.Select(p => p.ToBytes()))
            {
                Buffer.BlockCopy(permissionBytes, 0, bytes, offset, permissionBytes.Length);
                offset += permissionBytes.Length;
            }
            return permissions.SelectMany(p => p.ToBytes()).ToArray();
        }

        public static byte[] ToBytes(this IApiKeyPermission permission)
        {
            var bytes = new byte[ApiKeyPermissionBytes.TotalLength];
            BitConverter.GetBytes((ushort)permission.OperationId).CopyTo(bytes, ApiKeyPermissionBytes.Positions.OperationId);
            BitConverter.GetBytes((ushort)permission.ExternalEntityId).CopyTo(bytes, ApiKeyPermissionBytes.Positions.ExternalEntityId);
            BitConverter.GetBytes((ushort)(permission.LegalEntityId ?? 0)).CopyTo(bytes, ApiKeyPermissionBytes.Positions.LegalEntityId);
            return bytes;
        }

        public static IApiKeyPermission ToApiKeyPermission(this byte[] bytes)
        {
            return new ApiKeyPermission
            {
                OperationId = BitConverter.ToUInt16(bytes, ApiKeyPermissionBytes.Positions.OperationId),
                ExternalEntityId = BitConverter.ToUInt16(bytes, ApiKeyPermissionBytes.Positions.ExternalEntityId),
                LegalEntityId = ParseNonDefault(bytes, ApiKeyPermissionBytes.Positions.LegalEntityId),
            };
        }

        public static List<IApiKeyPermission> ToApiKeyPermissions(this byte[] bytes)
        {
            return bytes.ToBatches(ApiKeyPermissionBytes.TotalLength).Select(b => b.ToApiKeyPermission()).ToList();
        }

        private static ushort? ParseNonDefault(byte[] bytes, int index)
        {
            var parsedValue = BitConverter.ToUInt16(bytes, index);
            return parsedValue == default
                ? (ushort?)null
                : parsedValue;
        }

        private static List<byte[]> ToBatches(this byte[] bytes, int length)
        {

            var batches = bytes.Batch(length).Select(b =>b.ToArray()).ToList();

            if (batches.Last().Length != length)
            {
                throw new ApplicationException($"Byte array size is not a multiple of {length}");
            }

            return batches;
        }
    }
}
