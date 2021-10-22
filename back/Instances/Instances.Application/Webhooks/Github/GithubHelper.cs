using System.Linq;

namespace Instances.Application.Webhooks.Github
{
    internal static class GithubHelper
    {
        public static string GetBranchNameFromFullRef(this string fullRef)
        {
            if (string.IsNullOrWhiteSpace(fullRef))
            {
                return fullRef;
            }
            var splitRef = fullRef.Split('/');
            if (splitRef.Length < 3)
            {
                return fullRef;
            }
            // Une full ref de branche commence par refs/heads/
            return string.Join("/", splitRef.Skip(2));
        }
    }
}
