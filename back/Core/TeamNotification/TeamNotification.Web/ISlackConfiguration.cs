namespace TeamNotification.Web
{
    public interface ISlackConfiguration
    {
        public SlackHooks Hooks { get; }
    }

    public class SlackHooks
    {
        public string DemosMaintainers { get; set; }
        public string CloudControlTeam { get; set; }
        public string IpFilterRejectionAlert { get; set; }
    }
}
