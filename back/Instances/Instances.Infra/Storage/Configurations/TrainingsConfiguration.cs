using Instances.Domain.Trainings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instances.Infra.Storage.Configurations
{
    public class TrainingsConfiguration : IEntityTypeConfiguration<Training>
    {
        public void Configure(EntityTypeBuilder<Training> builder)
        {
            builder.ToTable("Trainings");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.EnvironmentId).HasColumnName("environmentId");
            builder.HasOne(d => d.Environment).WithMany().HasForeignKey(d => d.EnvironmentId);

            builder.Property(d => d.IsActive).HasColumnName("isActive");
            builder.Property(d => d.LastRestoredAt).HasColumnName("lastRestoredAt");

            builder.Property(d => d.AuthorId).HasColumnName("authorId");
            builder.HasOne(d => d.Author).WithMany().HasForeignKey(d => d.AuthorId);
            builder.Property(d => d.ApiKeyStorableId).HasColumnName("apiKeyStorableId");


            builder.Property(d => d.InstanceId).HasColumnName("instanceId");
            builder.HasOne(d => d.Instance).WithMany().HasForeignKey(d => d.InstanceId);

            builder.Property(d => d.TrainingRestorationId).HasColumnName("trainingRestorationId");
            builder.HasOne(d => d.TrainingRestoration).WithMany().HasForeignKey(d => d.TrainingRestorationId);

            builder.Ignore(d => d.Href);
        }
    }
}
