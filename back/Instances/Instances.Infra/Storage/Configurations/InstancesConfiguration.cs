using Instances.Domain.Instances.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instances.Infra.Storage.Configurations
{
    public class InstancesConfiguration : IEntityTypeConfiguration<Instance>
    {
        public void Configure(EntityTypeBuilder<Instance> builder)
        {
            builder.ToTable("Instances");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Cluster).HasColumnName("Cluster");
            builder.Property(d => d.Type).HasColumnName("Type");
            builder.Property(d => d.IsProtected).HasColumnName("IsProtected");
            builder.Property(d => d.IsAnonymized).HasColumnName("IsAnonymized");
            builder.Property(d => d.IsActive).HasColumnName("IsActive");
            builder.Property(d => d.AllUsersImposedPassword).HasColumnName("AllUsersImposedPassword");
            builder.Property(d => d.EnvironmentId).HasColumnName("EnvironmentId");
        }
    }
}
