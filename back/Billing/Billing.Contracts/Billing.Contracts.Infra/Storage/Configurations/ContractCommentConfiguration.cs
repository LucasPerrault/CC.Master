using Billing.Contracts.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Contracts.Infra.Storage.Configurations
{
    public class ContractCommentConfiguration : IEntityTypeConfiguration<ContractComment>
    {
        public void Configure(EntityTypeBuilder<ContractComment> builder)
        {
            builder.ToTable("ContractComments");
            builder.HasKey(d => d.ContractId);
            builder.Property(d => d.Comment).HasColumnName("Comment");
            builder.Property(d => d.DistributorId).HasColumnName("DistributorId");
        }
    }
}
