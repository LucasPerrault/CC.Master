using Distributors.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Storage.Infra.Context;

namespace Distributors.Infra.Storage.Configurations
{
    public class DistributorsConfiguration : IEntityTypeConfiguration<Distributor>
    {
        public void Configure(EntityTypeBuilder<Distributor> builder)
        {
            builder.ToTable("Distributors", StorageSchemas.Shared.Value);
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).HasColumnName("Name");
            builder.Property(d => d.Code).HasColumnName("Code");
            builder.Property(d => d.DepartmentId).HasColumnName("DepartmentId");
            builder.Property(d => d.IsAllowingCommercialCommunication).HasColumnName("IsAllowingCommercialCommunication");
            builder.Property(d => d.IsActive).HasColumnName("IsActive");
        }
    }
}
