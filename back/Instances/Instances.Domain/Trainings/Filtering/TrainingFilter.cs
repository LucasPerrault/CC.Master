using Tools;

namespace Instances.Domain.Trainings.Filtering
{
    public class TrainingFilter
    {
        public CompareBoolean IsActive { get; set; } = CompareBoolean.Bypass;
        public CompareBoolean IsProtected { get; set; } = CompareBoolean.Bypass;
        public CompareString Subdomain { get; set; } = CompareString.Bypass;
        public int? EnvironmentId { get; set; }
        public int? InstanceId { get; set; }

        public int? AuthorId { get; set; }

        public static TrainingFilter Active() => new TrainingFilter { IsActive = CompareBoolean.TrueOnly };
    }
}
