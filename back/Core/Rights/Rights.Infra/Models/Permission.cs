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
        public static byte[] ToBytes(this Permission permission)
        {
            return ToByteArrays(permission).SelectMany(b => b).ToArray();
        }
        public static byte[] ToBytes(this List<IUserPermission> permissions)
        {
            return permissions.SelectMany(ToByteArrays).SelectMany(b => b).ToArray();
        }

        private static IEnumerable<byte[]> ToByteArrays(IUserPermission permission)
        {
            yield return BitConverter.GetBytes((ushort)permission.OperationId);
            yield return BitConverter.GetBytes((ushort)permission.Scope);
            yield return BitConverter.GetBytes((ushort)permission.ExternalEntityId);
            yield return BitConverter.GetBytes((ushort)(permission.LegalEntityId ?? default));
            yield return BitConverter.GetBytes((ushort)(permission.SpecificUserId ?? default));
            yield return BitConverter.GetBytes((ushort)(permission.SpecificDepartmentId ?? default));
            yield return BitConverter.GetBytes(permission.HasContextualLegalEntityAssociation);
        }

        public static Permission ToPermission(this byte[] bytes)
        {
            return new Permission
            {
                OperationId = BitConverter.ToUInt16(bytes, 0),
                Scope = (Scope)BitConverter.ToUInt16(bytes, 2),
                ExternalEntityId = BitConverter.ToUInt16(bytes, 4),
                LegalEntityId = ParseNonDefault(bytes, 6),
                SpecificUserId = ParseNonDefault(bytes, 8),
                SpecificDepartmentId = ParseNonDefault(bytes, 10),
                HasContextualLegalEntityAssociation = BitConverter.ToBoolean(bytes, 12)
            };
        }

        public static List<Permission> ToPermissions(this byte[] bytes)
        {
            return bytes.ToBatches(13).Select(b => b.ToPermission()).ToList();
        }

        public static byte[] ToBytes(this IApiKeyPermission permission)
        {
            return ToByteArrays(permission).SelectMany(b => b).ToArray();
        }

        public static byte[] ToBytes(this List<IApiKeyPermission> permissions)
        {
            return permissions.SelectMany(ToByteArrays).SelectMany(b => b).ToArray();
        }

        private static IEnumerable<byte[]> ToByteArrays(IApiKeyPermission permission)
        {
            yield return BitConverter.GetBytes((ushort)permission.OperationId);
            yield return BitConverter.GetBytes((ushort)permission.ExternalEntityId);
            yield return BitConverter.GetBytes((ushort)(permission.LegalEntityId ?? 0));
        }

        public static IApiKeyPermission ToApiKeyPermission(this byte[] bytes)
        {
            return new ApiKeyPermission
            {
                OperationId = BitConverter.ToUInt16(bytes, 0),
                ExternalEntityId = BitConverter.ToUInt16(bytes, 2),
                LegalEntityId = ParseNonDefault(bytes, 4),
            };
        }

        public static List<IApiKeyPermission> ToApiKeyPermissions(this byte[] bytes)
        {
            return bytes.ToBatches(6).Select(b => b.ToApiKeyPermission()).ToList();
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
