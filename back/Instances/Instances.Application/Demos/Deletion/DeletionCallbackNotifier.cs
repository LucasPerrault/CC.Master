using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamNotification.Abstractions;

namespace Instances.Application.Demos.Deletion
{
    public class DeletionReport
    {
        public List<string> FailedInstances { get; set; }
        public List<string> SucceededInstances { get; set; }
    }

    public class DeletionCallbackNotifier
    {
        private readonly ITeamNotifier _teamNotifier;

        public DeletionCallbackNotifier(ITeamNotifier teamNotifier)
        {
            _teamNotifier = teamNotifier;
        }

        public async Task NotifyDemoDeletionReportAsync(string clusterName, DeletionReport report)
        {
            await _teamNotifier.NotifyAsync
            (
                Team.DemoMaintainers,
                $"Suppression de démos : {clusterName} annonce {report.SucceededInstances.Count} réussites."
            );

            if (report.FailedInstances.Any())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"Suppression de démos : {clusterName} annonce {report.FailedInstances.Count} échecs :");
                foreach (var failedInstance in report.FailedInstances)
                {
                    builder.AppendLine($"- {failedInstance}");
                }

                await _teamNotifier.NotifyAsync(Team.DemoMaintainers, builder.ToString());
            }
        }
    }
}
