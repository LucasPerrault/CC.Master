using System;

namespace Instances.Domain.Demos.Cleanup
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

        private static DemoState GetDemoState(Demo demo, DateTime today)
        {
            if (demo.DeletionScheduledOn <= today)
            {
                return DemoState.DeletionScheduledToday;
            }

            if (demo.DeletionScheduledOn < today.Add(ReportAheadDuration))
            {
                return DemoState.DeletionScheduledSoon;
            }

            return DemoState.AliveAndWell;
        }

        public DemoCleanupInfo(Demo demo, DateTime today)
        {
            Demo = demo;
            State = GetDemoState(demo, today);
        }

        public DemoCleanupInfo(Demo demo, Exception exception, DemoState errorState)
        {
            Demo = demo;
            State = errorState;
            Message = exception.Message;
        }
    }
}
