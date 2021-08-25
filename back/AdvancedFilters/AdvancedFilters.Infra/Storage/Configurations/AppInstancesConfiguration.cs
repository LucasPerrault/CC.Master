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
            builder.HasKey(e => e.Id);
            builder.Property(e => e.RemoteId).HasColumnName("RemoteId").IsRequired();
            builder.Property(e => e.Name).HasColumnName("Name").IsRequired();
            builder.Property(e => e.ApplicationId).HasColumnName("ApplicationId").IsRequired();
            builder.Property(e => e.EnvironmentId).HasColumnName("EnvironmentId").IsRequired();
            builder.Property(e => e.DeleteAt).HasColumnName("DeleteAt");

            builder.HasOne(e => e.Environment).WithMany().HasForeignKey(e => e.EnvironmentId);
        }
    }
}
