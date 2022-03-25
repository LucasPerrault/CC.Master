﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Tools
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            if (fi is null)
            {
                throw new InvalidEnumArgumentException($"Can't find {value.GetType()} enum member for value {value}");
            }

            if (fi?.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        public static bool ContainsAllEnumValues<T>(this IEnumerable<T> enums) where T : Enum
        {
            var allValues = Enum.GetValues(typeof(T)).Cast<T>().ToHashSet();
            return allValues.All(enums.Contains);
        }

        public static string[] GetAllEnumValuesExcept<T>(params T[] exception) where T : Enum
        {
            return EnumerateAllEnumsExcept(exception).Select(value => $"{value}").ToArray();
        }

        public static T[] GetAllEnumsExcept<T>(params T[] exception) where T : Enum
        {
            return EnumerateAllEnumsExcept(exception).ToArray();
        }

        private static IEnumerable<T> EnumerateAllEnumsExcept<T>(params T[] exception) where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Except(exception);
        }
    }
}
