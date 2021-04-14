using Distributors.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Distributors.Infra.Storage.Configurations
{
    public class DistributorsConfiguration : IEntityTypeConfiguration<Distributor>
    {
        public void Configure(EntityTypeBuilder<Distributor> builder)
        {
            builder.ToView("Distributors");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).HasColumnName("Name");
            builder.Property(d => d.Code).HasColumnName("Code");
            builder.Property(d => d.DepartmentId).HasColumnName("DepartmentId");
        }
    }
}
