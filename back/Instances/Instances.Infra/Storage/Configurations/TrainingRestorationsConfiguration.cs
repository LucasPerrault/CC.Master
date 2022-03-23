using Instances.Domain.Trainings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instances.Infra.Storage.Configurations
{
    public class TrainingRestorationsConfiguration : IEntityTypeConfiguration<TrainingRestoration>
    {
        public void Configure(EntityTypeBuilder<TrainingRestoration> builder)
        {
            builder.ToTable("TrainingRestorations");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.EnvironmentId).HasColumnName("environmentId");

            builder.Property(d => d.Anonymize).HasColumnName("anonymize");
            builder.Property(d => d.ApiKeyStorableId).HasColumnName("apiKeyStorableId");
            builder.Property(d => d.AuthorId).HasColumnName("authorId");
            builder.Property(d => d.Comment).HasColumnName("comment");
            builder.Property(d => d.CommentExpiryDate).HasColumnName("commentExpiryDate");
            builder.Property(d => d.InstanceDuplicationId).HasColumnName("instanceDuplicationId");
            builder.HasOne(d => d.InstanceDuplication).WithMany().HasForeignKey(d => d.InstanceDuplicationId);
            builder.Property(d => d.KeepExistingTrainingPasswords).HasColumnName("keepExistingTrainingPassword");
            builder.Property(d => d.RestoreFiles).HasColumnName("restoreFiles");

            builder.Ignore(d => d.DistributorId);
            builder.Ignore(d => d.HasEnded);
        }
    }
}
