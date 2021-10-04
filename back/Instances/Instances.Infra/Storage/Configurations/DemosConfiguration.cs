using Instances.Domain.Demos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instances.Infra.Storage.Configurations
{
    public class DemosConfiguration : IEntityTypeConfiguration<Demo>
    {
        public void Configure(EntityTypeBuilder<Demo> builder)
        {
            builder.ToTable("Demos");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Subdomain).HasColumnName("subdomain");
            builder.Property(d => d.CreatedAt).HasColumnName("createdAt");
            builder.Property(d => d.DeletionScheduledOn).HasColumnName("deletionScheduledOn");
            builder.Property(d => d.IsActive).HasColumnName("isActive");
            builder.Property(d => d.Cluster).HasColumnName("Cluster");

            builder.Property(d => d.AuthorId).HasColumnName("authorId");
            builder.HasOne(d => d.Author).WithMany().HasForeignKey(d => d.AuthorId);

            builder.Property(d => d.InstanceID).HasColumnName("instanceId");
            builder.HasOne(d => d.Instance).WithOne().HasForeignKey<Demo>(d => d.InstanceID);

            builder.Property(d => d.DistributorId).HasColumnName("distributorID");

            builder.Property(d => d.Comment).HasColumnName("comment");
        }
    }
}
