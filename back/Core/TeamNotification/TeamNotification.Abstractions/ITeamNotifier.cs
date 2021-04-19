using System.Threading.Tasks;

namespace TeamNotification.Abstractions
{
    public interface ITeamNotifier
    {
        Task NotifyAsync(Team team, string message);
    }
}
