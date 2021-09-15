using System.Collections.Generic;
using System.Linq;

namespace Storage.Infra.Querying
{
    public static class FullTextHelper
    {
        private const string Quote = "\"";
        private static readonly string ClueSeparator = $"{Quote} AND {Quote}";

        public static string ToFullTextContainsPredicate(this HashSet<string> clues)
        {
            var cluesWithAsterisk = SanitizeClues(clues).Select(c => $"{c}*");
            var clue = string.Join(ClueSeparator, cluesWithAsterisk);
            return $"{Quote}{clue}{Quote}";
        }

        private static IEnumerable<string> SanitizeClues(HashSet<string> words)
        {
            foreach (var word in words ?? new HashSet<string>())
            {
                yield return ContainsAlphaNumeric(word)
                    ? EscapeSpecialCharacters(word)
                    : word;
            }
        }

        private static bool ContainsAlphaNumeric(string item)
        {
            return item.ToCharArray().Any(char.IsLetterOrDigit);
        }

        private static string EscapeSpecialCharacters(string item)
        {
            return item.Replace("\"", "\"\"");
        }
    }

}
