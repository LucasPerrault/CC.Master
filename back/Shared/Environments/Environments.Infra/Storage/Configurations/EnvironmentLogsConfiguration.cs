using Environments.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Infra.Context;

namespace Environments.Infra.Storage.Configurations
{
    public class EnvironmentLogsConfiguration : IEntityTypeConfiguration<EnvironmentLog>
    {
        public void Configure(EntityTypeBuilder<EnvironmentLog> builder)
        {
            builder.ToTable("EnvironmentLogs", StorageSchemas.Shared.Value);
            builder.HasKey(d => d.Id);
            builder.Property(d => d.UserId).HasColumnName("userId");
            builder.Property(d => d.ActivityId).HasColumnName("activityId");
            builder.Property(d => d.EnvironmentId).HasColumnName("environmentId");
            builder.Property(d => d.CreatedOn).HasColumnName("createdOn");
            builder.Property(d => d.IsAnonymizedData).HasColumnName("IsAnonymizedData");
            builder.HasMany(d => d.Messages).WithOne().HasForeignKey(m => m.EnvironmentLogId);
            builder.Ignore(d => d.Activity);
        }
    }

    public class EnvironmentLogMessagesConfiguration : IEntityTypeConfiguration<EnvironmentLogMessage>
    {
        public void Configure(EntityTypeBuilder<EnvironmentLogMessage> builder)
        {
            builder.ToTable("EnvironmentLogMessages", StorageSchemas.Shared.Value);
            builder.HasKey(d => d.Id);
            builder.Property(d => d.EnvironmentLogId).HasColumnName("environmentLogId");
            builder.Property(d => d.UserId).HasColumnName("userId");
            builder.Property(d => d.CreatedOn).HasColumnName("createdOn");
            builder.Property(d => d.ExpiredOn).HasColumnName("expiredOn");
            builder.Property(d => d.Message).HasColumnName("message");
            builder.Property(d => d.TypeString).HasColumnName("type");

            builder.Ignore(d => d.Name);
            builder.Ignore(d => d.Type);
        }
    }

}
