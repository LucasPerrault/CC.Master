using AdvancedFilters.Domain.Billing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedFilters.Infra.Storage.Configurations
{
    public class ContractsConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.ToTable("Contracts");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.RemoteId).HasColumnName("RemoteId").IsRequired();
            builder.Property(c => c.ExternalId).HasColumnName("ExternalId").HasMaxLength(36).IsRequired();
            builder.Property(c => c.ClientId).HasColumnName("ClientId").IsRequired();

            builder.HasOne(c => c.Client).WithMany().HasPrincipalKey(cl => cl.RemoteId).HasForeignKey(c => c.ClientId);

            builder.HasIndex(ai => ai.RemoteId).IsUnique();
        }
    }
}
