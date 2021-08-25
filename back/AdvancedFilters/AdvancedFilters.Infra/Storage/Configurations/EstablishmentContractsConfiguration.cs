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
            builder.Property(ec => ec.EstablishmentId).HasColumnName("EstablishmentId").IsRequired();
            builder.Property(ec => ec.ContractId).HasColumnName("ContractId").IsRequired();
            // TODO FKs
        }
    }
}
