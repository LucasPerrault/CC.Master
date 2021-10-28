using AdvancedFilters.Domain.Instance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedFilters.Infra.Storage.Configurations
{
    public class EnvironmentAccessesConfiguration : IEntityTypeConfiguration<EnvironmentAccess>
    {
        public void Configure(EntityTypeBuilder<EnvironmentAccess> builder)
        {
            builder.ToTable("EnvironmentAccesss");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedNever();

            builder.Property(c => c.EnvironmentId).HasColumnName("EnvironmentId").IsRequired();
            builder.Property(c => c.DistributorId).HasColumnName("DistributorId").IsRequired();

            builder.HasIndex(e => e.EnvironmentId);
            builder.HasIndex(e => e.DistributorId);

            builder
                .HasOne(a => a.Environment)
                .WithMany(e => e.Accesses)
                .HasForeignKey(a => a.EnvironmentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasOne(a => a.Distributor)
                .WithMany(d => d.EnvironmentAccesses)
                .HasForeignKey(a => a.DistributorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
