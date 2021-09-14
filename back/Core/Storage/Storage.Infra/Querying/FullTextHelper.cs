using System.Collections.Generic;
using System.Linq;

namespace Storage.Infra.Querying
{
    public static class FullTextHelper
    {
        private static IEnumerable<string> SanitizeClue(HashSet<string> words)
        {
            foreach (var word in words ?? new HashSet<string>())
            {
                if (ContainsAlphaNumeric(word))
                {
                    yield return EscapeSpecialCharacters(word);
                }

                yield return word;
            }
        }

        public static string ToFullTextContainsPredicate(this HashSet<string> clue)
        {
            var sanitizedClue = SanitizeClue(clue);
            return "\"" + string.Join("*\" AND \"", sanitizedClue) + "*\"";
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
