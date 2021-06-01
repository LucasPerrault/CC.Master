using Environments.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Infra.Context;

namespace Environments.Infra.Storage.Configurations
{
    public class EnvironmentAccessesConfiguration : IEntityTypeConfiguration<EnvironmentAccess>
    {
        public void Configure(EntityTypeBuilder<EnvironmentAccess> builder)
        {
            builder.ToTable("EnvironmentAccesses", StorageSchemas.Shared.Value);
            builder.HasKey(d => d.Id);
            builder.Property(d => d.DistributorId).HasColumnName("DistributorId");
            builder.Property(d => d.EnvironmentId).HasColumnName("EnvironmentId");

            builder.Property(d => d.AuthorId).HasColumnName("AuthorId");
            builder.Property(d => d.Type).HasColumnName("Type");

            builder.Property(d => d.StartsAt).HasColumnName("StartsAt");
            builder.Property(d => d.EndsAt).HasColumnName("EndsAt");
            builder.Property(d => d.RevokedAt).HasColumnName("RevokedAt");

            builder.Property(d => d.Comment).HasColumnName("Comment");
            builder.Property(d => d.RevocationComment).HasColumnName("RevocationComment");
            builder.Property(d => d.Lifecycle).HasComputedColumnSql("Lifecycle");

        }
    }

    public class EnvironmentSharedAccessesConfiguration : IEntityTypeConfiguration<EnvironmentSharedAccess>
    {
        public void Configure(EntityTypeBuilder<EnvironmentSharedAccess> builder)
        {
            builder.ToTable("EnvironmentSharedAccesses", StorageSchemas.Shared.Value);
            builder.HasKey(d => d.Id);
            builder.Property(d => d.EnvironmentId).HasColumnName("EnvironmentId");
            builder.Property(d => d.ConsumerId).HasColumnName("ConsumerId");

            builder.HasOne(d => d.Access).WithOne().HasForeignKey<EnvironmentSharedAccess>(d => d.Id);
            builder.HasOne(d => d.Consumer).WithOne().HasForeignKey<EnvironmentSharedAccess>(d => d.ConsumerId);
        }
    }
}
