using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Instances.Infra.Shared
{
    internal class ParsedCluster
    {
        public string Name { get; set; }
        public int? Number { get; set; }
    }


    // Changements à reporter dans le CClithe
    internal static class ClusterNameConvertor
    {
        private static readonly Regex ClusterNumberExtractor = new Regex(@"([a-z]+)(\d+)", RegexOptions.Compiled);

        public static string GetLongName(string clusterName)
        {
            return GetLongName(GetParsedCluster(clusterName));
        }

        public static string GetLongName(ParsedCluster parsedCluster)
        {
            return parsedCluster.Name + GetClusterNumber(parsedCluster);
        }

        public static string GetShortName(string clusterName)
        {
            return GetShortName(GetParsedCluster(clusterName));
        }

        public static string GetShortName(ParsedCluster parsedCluster)
        {
            return GetShortNamePrefix(parsedCluster) + GetClusterNumber(parsedCluster);
        }

        public static string GetSpecificDomain(string cluster)
        {
            return cluster.ToLowerInvariant() switch
            {
                "ch1" => "lucca-ch.local",
                _ => null
            };
        }

        private static string GetShortNamePrefix(ParsedCluster context)
        {
            return context.Name switch
            {
                "ch" => "ch",
                "cluster" => "c",
                "demo" => "dm",
                "preview" => "pw",
                "formation" => "fm",
                "test" => "fm",
                "security" => "se",
                "recette" => "re",
                _ => throw new NotSupportedException($"Cluster name is not supported : {context.Name}")
            };
        }

        private static string GetClusterNumber(ParsedCluster context)
        {
            return context.Name switch
            {
                "test" => string.Empty,
                "demo" when !context.Number.HasValue => "1",
                _ => context.Number.HasValue ? context.Number.ToString() : string.Empty
            };
        }

        private static ParsedCluster GetParsedCluster(string cluster)
        {
            cluster = cluster.ToLowerInvariant();
            var match = ClusterNumberExtractor.Match(cluster);

            if (match.Success)
            {
                return new ParsedCluster
                {
                    Name = match.Groups[1].Value,
                    Number = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture)
                };
            }

            return new ParsedCluster
            {
                Name = cluster
            };
        }
    }
}
