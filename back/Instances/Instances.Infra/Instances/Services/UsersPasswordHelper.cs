using Instances.Domain.Instances;
using Instances.Infra.Resources;
using Lucca.Core.Shared.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Instances.Infra.Instances.Services
{
    public class UsersPasswordHelper : IUsersPasswordHelper
    {
        private static readonly Regex Regex = new Regex
        (
            @"^[a-z0-9]+$",
            RegexOptions.IgnoreCase
        );

        private static readonly HashSet<string> TerriblePasswordsAllowedForLegacyReasons = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "test"
        };

        private static readonly Random Random = new Random();

        public const int MinLength = 6;
        public const int MaxLength = 255;

        public void ThrowIfInvalid(string password)
        {
            if(string.IsNullOrEmpty(password))
            {
                throw new BadRequestException($"Password empty, you must specify a password");
            }

            if (TerriblePasswordsAllowedForLegacyReasons.Contains(password))
            {
                return;
            }

            if (password.Length < MinLength)
            {
                throw new BadRequestException($"Password is too short (min {MinLength} characters)");
            }
            if (password.Length > MaxLength)
            {
                throw new BadRequestException($"Password is too short (max {MaxLength} characters)");
            }
            if (!Regex.IsMatch(password))
            {
                throw new BadRequestException("Password does not match requested format.");
            }
        }

        public string Generate()
        {
            var randomCity = PasswordResources.Prefixes[Random.Next(PasswordResources.Prefixes.Count)];
            return randomCity + Random.Next(9999).ToString("D4");
        }
    }
}
