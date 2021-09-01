using AdvancedFilters.Domain.Billing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedFilters.Infra.Storage.Configurations
{
    class EstablishmentContractsConfiguration : IEntityTypeConfiguration<EstablishmentContract>
    {
        public void Configure(EntityTypeBuilder<EstablishmentContract> builder)
        {
            builder.ToTable("EstablishmentContracts");
            builder.HasKey(ec => new { ec.ContractId, ec.EstablishmentId });
            builder.Property(ec => ec.EstablishmentId).HasColumnName("EstablishmentId").IsRequired();
            builder.Property(ec => ec.ContractId).HasColumnName("ContractId").IsRequired();

            builder.HasOne(ec => ec.Contract).WithMany().HasPrincipalKey(c => c.RemoteId).HasForeignKey(ec => ec.ContractId);
            builder.HasOne(ec => ec.Establishment).WithMany().HasPrincipalKey(e => e.RemoteId).HasForeignKey(ec => ec.EstablishmentId);
        }
    }
}
