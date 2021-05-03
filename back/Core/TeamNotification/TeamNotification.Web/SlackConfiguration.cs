using System;

namespace TeamNotification.Web
{
    public class SlackConfiguration
    {
        public SlackHooks Hooks { get; set; }
    }

    public class SlackHooks
    {
        public Uri DemosMaintainers { get; set; }
        public Uri CloudControlTeam { get; set; }
    }
}
