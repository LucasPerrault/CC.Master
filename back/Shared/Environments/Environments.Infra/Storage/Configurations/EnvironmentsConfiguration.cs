using Environments.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Infra.Context;

namespace Environments.Infra.Storage.Configurations
{
    public class EnvironmentsConfiguration : IEntityTypeConfiguration<Environment>
    {
        public void Configure(EntityTypeBuilder<Environment> builder)
        {
            builder.ToTable("Environments", StorageSchemas.Shared.Value);
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Subdomain).HasColumnName("Subdomain");
            builder.Property(d => d.Domain).HasColumnName("Domain");
            builder.Property(d => d.Cluster).HasColumnName("Cluster");
            builder.Property(d => d.Purpose).HasColumnName("Purpose");
            builder.Property(d => d.IsActive).HasColumnName("IsActive");
            builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt");

            builder.HasMany(d => d.ActiveAccesses).WithOne().HasForeignKey(d => d.EnvironmentId);

            builder.Ignore(d => d.ProductionHost);
        }
    }
}
