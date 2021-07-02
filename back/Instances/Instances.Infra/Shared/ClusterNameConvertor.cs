using System;
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

        private static string GetShortNamePrefix(ParsedCluster context)
        {
            return context.Name switch
            {
                "cluster" => "c",
                "demo" => "dm",
                "preview" => "pw",
                "formation" => "fm",
                "green" => "ch",
                "security" => "se",
                "recette" => "re",
                _ => throw new NotSupportedException($"Cluster name is not supported : {context.Name}")
            };
        }

        private static string GetClusterNumber(ParsedCluster context)
        {
            return context.Name switch
            {
                "green" => string.Empty,
                "demo" when !context.Number.HasValue => "1",
                _ => context.Number.HasValue ? context.Number.ToString() : string.Empty
            };
        }

        private static ParsedCluster GetParsedCluster(string cluster)
        {
            cluster = cluster.ToLower();
            var match = ClusterNumberExtractor.Match(cluster);

            if (match.Success)
            {
                return new ParsedCluster
                {
                    Name = match.Groups[1].Value,
                    Number = int.Parse(match.Groups[2].Value)
                };
            }

            return new ParsedCluster
            {
                Name = cluster
            };
        }
    }
}
