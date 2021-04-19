using Remote.Infra.Extensions;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TeamNotification.Abstractions;

namespace TeamNotification.Web
{
    public class TeamNotifier : ITeamNotifier
    {
        private readonly SlackHooks _hooks;
        private readonly HttpClient _httpClient;

        public TeamNotifier(SlackHooks hooks, HttpClient httpClient)
        {
            _hooks = hooks;
            _httpClient = httpClient;
        }

        public Task NotifyAsync(Team team, string message)
        {
            var slackHook = GetHook(team);
            var content = new { Text = message };
            return _httpClient.PostAsync(slackHook, content.ToJsonPayload());
        }

        private Uri GetHook(Team team)
        {
            return team switch
            {
                Team.DemoMaintainers => _hooks.DemosMaintainers,
                _ => _hooks.CloudControlTeam // a notification should never be lost, CC team is default team
            };
        }
    }
}
