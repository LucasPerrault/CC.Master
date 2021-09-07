using AdvancedFilters.Domain.Billing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvancedFilters.Infra.Storage.Configurations
{
    public class ClientsConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Clients");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.ExternalId).HasColumnName("ExternalId").HasMaxLength(36).IsRequired();
            builder.Property(c => c.Name).HasColumnName("Name").IsRequired();

            builder.HasMany(c => c.Contracts).WithOne().HasForeignKey(co => co.ClientId);

            builder.HasIndex(e => e.ExternalId).IsUnique();
        }
    }
}
