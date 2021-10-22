namespace Instances.Application.Webhooks.Github
{
    // https://developer.github.com/v3/activity/events/types/
    public enum GithubEvent
    {
        NotSupported,
        Push,
        Pull_Request
    }
}
