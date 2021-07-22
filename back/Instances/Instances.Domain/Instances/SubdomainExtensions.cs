using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Instances.Domain.Instances
{
    public static class SubdomainExtensions
    {
        public const int SubdomainMinLength = 2;
        public const int SubdomainMaxLength = 63;

        private const char Separator = '-';
        public static string ToValidSubdomain(this string s)
        {
            return s
                .SeparateDiacriticsFromLetters()
                .ToLower()
                .ToCharArray()
                .Select(c => char.IsWhiteSpace(c) ? Separator : c)
                .Where(c => char.IsLetterOrDigit(c) || c == Separator)
                .WithoutConsecutiveChars(Separator)
                .WithoutFirstChars(Separator)
                .WithoutLastChars(Separator)
                .Take(SubdomainMaxLength)
                .Concat();
        }

        // https://fr.wikipedia.org/wiki/Normalisation_Unicode
        private static string SeparateDiacriticsFromLetters(this string text)
        {
            return text.Normalize(NormalizationForm.FormD);
        }

        private static string Concat(this IEnumerable<char> chars)
        {
            return string.Concat(chars);
        }

        private static IEnumerable<char> WithoutConsecutiveChars(this IEnumerable<char> chars, params char[] charsWithoutConsecutiveRepetition)
        {
            char lastYieldedChar = default;
            bool isFirst = true;

            foreach (var c in chars)
            {
                if (!isFirst && charsWithoutConsecutiveRepetition.Contains(c) && lastYieldedChar == c)
                {
                    continue;
                }

                yield return c;
                isFirst = false;
                lastYieldedChar = c;
            }
        }

        private static IEnumerable<char> WithoutFirstChars(this IEnumerable<char> chars, params char[] charsThatShouldNotBeFirst)
        {
            bool isFirstYieldedChar = true;

            foreach (var c in chars)
            {
                if (isFirstYieldedChar && charsThatShouldNotBeFirst.Contains(c))
                {
                    continue;
                }

                yield return c;
                isFirstYieldedChar = false;
            }
        }

        private static IEnumerable<char> WithoutLastChars(this IEnumerable<char> chars, params char[] charsThatShouldNotBeLast)
        {
            return chars
                .Reverse()
                .WithoutFirstChars(charsThatShouldNotBeLast)
                .Reverse();
        }
    }
}
