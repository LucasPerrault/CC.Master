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
            builder.HasKey(ai => new { ai.EnvironmentId, ai.Id });

            builder.Property(ai => ai.Name).HasColumnName("Name").IsRequired();
            builder.Property(ai => ai.ApplicationId).HasColumnName("ApplicationId").IsRequired();
            builder.Property(ai => ai.DeletedAt).HasColumnName("DeletedAt");

            builder.Ignore(ai => ai.ApplicationName);

            builder
                .HasOne(ai => ai.Environment)
                .WithMany(e => e.AppInstances)
                .HasForeignKey(ai => ai.EnvironmentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
