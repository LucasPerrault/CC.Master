namespace TeamNotification.Web
{
    public class SlackConfiguration
    {
        public SlackHooks Hooks { get; set; }
    }

    public class SlackHooks
    {
        public string DemosMaintainers { get; set; }
        public string CloudControlTeam { get; set; }
    }
}
