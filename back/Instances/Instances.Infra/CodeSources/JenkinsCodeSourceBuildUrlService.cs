using Instances.Domain.CodeSources;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Instances.Infra.CodeSources
{
    public class JenkinsCodeSourceBuildUrlService : ICodeSourceBuildUrlService
    {
        private readonly HttpClient _httpClient;

        public JenkinsCodeSourceBuildUrlService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public bool IsValidBuildNumber(string builderNumber)
        {
            if (string.IsNullOrEmpty(builderNumber))
            {
                return true;
            }
            if (builderNumber == "lastSuccessfulBuild" || builderNumber == "lastBuild")
            {
                return true;
            }
            return int.TryParse(builderNumber, out _);
        }

        public async Task<string> GenerateBuildUrlAsync(CodeSource codeSource, string branchName, string buildNumber)
        {
            var sb = new StringBuilder(codeSource.JenkinsProjectUrl);
            if (codeSource.JenkinsProjectUrl[codeSource.JenkinsProjectUrl.Length - 1] == '/')
            {
                --sb.Length;
            }
            sb.Append("/job/");
            sb.Append(branchName);
            sb.Append("/");
            if (string.IsNullOrEmpty(buildNumber))
            {
                sb.Append(await GetBuildNumberAsync(sb.ToString(), "lastSuccessfulBuild"));
            }
            else if (int.TryParse(buildNumber, out _))
            {
                sb.Append(buildNumber);
            }
            else
            {
                sb.Append(await GetBuildNumberAsync(sb.ToString(), buildNumber));
            }
            return sb.ToString();
        }

        private async Task<string> GetBuildNumberAsync(string baseUri, string type)
        {
            var response = await _httpClient.GetAsync($"{baseUri}api/json");
            response.EnsureSuccessStatusCode();

            await using var body = await response.Content.ReadAsStreamAsync();

            var doc = await JsonDocument.ParseAsync(body);
            return doc.RootElement.GetProperty(type).GetProperty("number").GetInt32().ToString();
        }
    }
}
