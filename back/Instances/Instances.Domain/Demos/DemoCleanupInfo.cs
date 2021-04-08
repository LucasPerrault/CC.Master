using System;

namespace Instances.Domain.Demos
{
    public enum DemoState
    {
        AliveAndWell,
        DeletionScheduledToday,
        DeletionScheduledSoon,
        ErrorAtStateEvaluation
    }

    public class DemoCleanupInfo
    {
        public static readonly TimeSpan ReportAheadDuration = TimeSpan.FromDays(9);

        public Demo Demo { get; }
        public DemoState State { get; }
        public string Message { get; }

        public DateTime DeletionScheduledDate => Demo.DeletionScheduledOn;

        private static DemoState GetDemoState(Demo demo)
        {
            if (demo.DeletionScheduledOn <= DateTime.Today)
            {
                return DemoState.DeletionScheduledToday;
            }

            if (demo.DeletionScheduledOn < DateTime.Today.Add(ReportAheadDuration))
            {
                return DemoState.DeletionScheduledSoon;
            }

            return DemoState.AliveAndWell;
        }

        public DemoCleanupInfo(Demo demo)
        {
            Demo = demo;
            State = GetDemoState(demo);
        }

        public DemoCleanupInfo(Demo demo, Exception exception, DemoState errorState)
        {
            Demo = demo;
            State = errorState;
            Message = exception.Message;
        }

        public bool ShouldBeReported()
        {
            return State != DemoState.AliveAndWell;
        }
    }
}
