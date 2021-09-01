using AdvancedFilters.Domain.Instance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedFilters.Infra.Storage.Configurations
{
    public class AppInstancesConfiguration : IEntityTypeConfiguration<AppInstance>
    {
        public void Configure(EntityTypeBuilder<AppInstance> builder)
        {
            builder.ToTable("AppInstances");
            builder.HasKey(ai => ai.Id);
            builder.Property(ai => ai.RemoteId).HasColumnName("RemoteId").IsRequired();
            builder.Property(ai => ai.Name).HasColumnName("Name").IsRequired();
            builder.Property(ai => ai.ApplicationId).HasColumnName("ApplicationId").IsRequired();
            builder.Property(ai => ai.EnvironmentId).HasColumnName("EnvironmentId").IsRequired();
            builder.Property(ai => ai.DeleteAt).HasColumnName("DeleteAt");

            builder.HasOne(ai => ai.Environment).WithMany().HasPrincipalKey(e => e.RemoteId).HasForeignKey(ai => ai.EnvironmentId);

            builder.HasIndex(ai => ai.RemoteId).IsUnique();
            builder.HasIndex(ai => ai.EnvironmentId);
        }
    }
}
