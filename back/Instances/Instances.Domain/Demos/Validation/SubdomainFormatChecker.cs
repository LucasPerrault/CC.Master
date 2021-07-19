using Instances.Domain.Instances;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Instances.Domain.Demos.Validation
{
    public enum SubdomainValidity
    {
        Ok,
        ReservedWord,
        ReservedPrefix,
        TooShort,
        TooLong,
        WrongFormat
    }

    public static class SubdomainFormatChecker
    {
        public static SubdomainValidity GetValidity(string subdomain)
        {
            if (SubdomainFormatConstants.ReservedSubdomains.Contains(subdomain))
            {
                return SubdomainValidity.ReservedWord;
            }
            if (SubdomainFormatConstants.ReservedSubdomainPrefixes.Any(subdomain.StartsWith))
            {
                return SubdomainValidity.ReservedPrefix;
            }
            if (subdomain.Length < SubdomainExtensions.SubdomainMinLength)
            {
                return SubdomainValidity.TooShort;
            }
            if (subdomain.Length > SubdomainExtensions.SubdomainMaxLength)
            {
                return SubdomainValidity.TooLong;
            }
            if (!Regex.IsMatch(subdomain, SubdomainFormatConstants.DemoRegex))
            {
                return SubdomainValidity.WrongFormat;
            }

            return SubdomainValidity.Ok;
        }
    }

    public static class SubdomainFormatConstants
    {
        internal const string DemoRegex = @"^(?!-)[a-z0-9-]+(?<!-)$";

        internal static readonly string[] ReservedSubdomains = {
            "cc", "cc-preview", "cc-training", "certificat", "adfs", "test-adfs", "dev", "apps","app", "api", "cdn",
            "ccnet", "versioningservice", "creation-client", "dashboard", "maintenance", "partenaires", "pdfservice",
            "planificateur", "sync", "figgo", "utime", "pagga", "urba", "ilucca", "ping", "pm", "sms", "www",
            "ws-cluster", "login"
        };

        internal static readonly string[] ReservedSubdomainPrefixes =
            { "ovh", "static", "adfs", "ws", "cleemy", "poplee", "timmi", "fake-", "iis-" };
    }
}
