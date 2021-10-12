using Instances.Application.Webhooks.Github;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static Instances.Web.InstancesConfigurer;

namespace Instances.Web.Webhooks
{
    public class GithubWebhookHandler
    {
        private const string GithubEventHeader = "x-github-event";
        public const string X_HUB_SIGNATURE = "X-Hub-Signature";

        private readonly IGithubWebhookServiceProvider _githubWebhookServiceProvider;
        private readonly InstancesConfiguration _configuration;

        public GithubWebhookHandler(IGithubWebhookServiceProvider githubWebhookServiceProvider, InstancesConfiguration configuration)
        {
            _githubWebhookServiceProvider = githubWebhookServiceProvider;
            _configuration = configuration;
        }

        public async Task<IActionResult> HandleGithubAsync(HttpRequest request)
        {
            request.EnableBuffering();
            if (!await IsAuthorizedAsync(request))
            {
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
            var githubEvent = GetGithubEvent(request.Headers);
            var githubWebhookService = _githubWebhookServiceProvider.GetGithubWebhookService(githubEvent);
            if (githubWebhookService == null)
            {
                return new StatusCodeResult(StatusCodes.Status204NoContent);
            }
            await githubWebhookService.HandleEventAsync(request.Body);
            return new StatusCodeResult(StatusCodes.Status201Created);
        }

        private async Task<bool> IsAuthorizedAsync(HttpRequest request)
        {
            try
            {
                if (!request.Headers.TryGetValue(X_HUB_SIGNATURE, out var authHeaders)) {
                    return false;
                }
                var authHeader = authHeaders.FirstOrDefault();

                if (string.IsNullOrEmpty(authHeader))
                {
                    return false;
                }

                return await IsValidTokenAsync(authHeader, request);
            }
            catch (Exception) { }
            finally
            {
                request.Body.Position = 0;
            }
            return false;
        }

        ///// <summary>
        ///// http://chris.59north.com/post/Integrating-with-Github-Webhooks-using-OWIN
        ///// </summary>
        ///// <returns></returns>
        private async Task<bool> IsValidTokenAsync(string payloadToken, HttpRequest request)
        {
            //Get token stored on enviroment
            var serverToken = _configuration.Github.GithubWebhookSecret;

            //Need to get the actual content of the request - JSON payload
            //As this payload is signed/encoded with our key
            var reader = new StreamReader(request.Body);
            var jsonPayload = await reader.ReadToEndAsync();
            
            //Verify the payloadToken starts with sha1
            var vals = payloadToken.Split('=');
            if (vals[0] != "sha1")
            {
                return false;
            }


            var encoding = new System.Text.UTF8Encoding();
            var keyByte = encoding.GetBytes(serverToken);
            string hash = null;
            using (var hmacsha1 = new HMACSHA1(keyByte))
            {
                var messageBytes = encoding.GetBytes(jsonPayload);
                var hashmessage = hmacsha1.ComputeHash(messageBytes);
                hash = hashmessage.Aggregate("", (current, t) => current + t.ToString("x2"));
            }
            return hash.Equals(vals[1], StringComparison.OrdinalIgnoreCase);
        }

        private static GithubEvent GetGithubEvent(IHeaderDictionary headers)
        {
            var caseInsensitiveHeaders = new Dictionary<string, StringValues>(headers, StringComparer.InvariantCultureIgnoreCase);
            if (!caseInsensitiveHeaders.TryGetValue(GithubEventHeader, out var eventHeaderValue))
            {
                return GithubEvent.NotSupported;
            }
            var rawGithubEvent = eventHeaderValue.FirstOrDefault();

            if (!Enum.TryParse(rawGithubEvent, ignoreCase: true, out GithubEvent githubEvent))
            {
                githubEvent = GithubEvent.NotSupported;
            }
            return githubEvent;
        }

    }
}
