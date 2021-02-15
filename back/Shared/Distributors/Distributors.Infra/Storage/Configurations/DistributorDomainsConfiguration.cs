using Distributors.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Distributors.Infra.Storage.Configurations
{
    public class DistributorDomainsConfiguration : IEntityTypeConfiguration<DistributorDomain>
    {
        public void Configure(EntityTypeBuilder<DistributorDomain> builder)
        {
            builder.ToView("DistributorDomains");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Domain).HasColumnName("Domain");
            builder.Property(d => d.DistributorId).HasColumnName("DistributorId");
        }
    }
}
