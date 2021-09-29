using Microsoft.AspNetCore.Http;

namespace Rights.Web
{
    public static class BetaTesterHelper
    {

        private const string BetaTesterKey = "IsCurrentUserABetaTester";
        public static bool IsBetaTester(HttpContext context)
        {
            context.Items.TryGetValue(BetaTesterKey, out var isBetaTester);
            return isBetaTester != null && (bool)isBetaTester;
        }

        public static void SetBetaTester(HttpContext context, bool isBetaTester)
        {
            context.Items[BetaTesterKey] = isBetaTester;
        }
    }
}
