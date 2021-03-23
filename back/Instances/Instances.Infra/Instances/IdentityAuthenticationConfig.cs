namespace Instances.Infra.Instances
{
    public class IdentityAuthenticationConfig
    {
        public const string ImpersonationScope = "api-identity impersonation";

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TokenRequestRoute { get; set; }
    }
}
