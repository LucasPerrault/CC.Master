using Billing.Contracts.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Contracts.Infra.Storage.Configurations
{
    public class EstablishmentAttachmentConfiguration : IEntityTypeConfiguration<EstablishmentAttachment>
    {
        public void Configure(EntityTypeBuilder<EstablishmentAttachment> builder)
        {
            builder.ToTable("EstablishmentAttachments");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.ContractId).HasColumnName("ContractId").IsRequired();
            builder.Property(d => d.EstablishmentId).HasColumnName("EstablishmentId").IsRequired();
            builder.Property(d => d.EstablishmentName).HasColumnName("EstablishmentName").IsRequired();
            builder.Property(d => d.EstablishmentRemoteId).HasColumnName("EstablishmentRemoteId").IsRequired();
            builder.Property(d => d.StartsOn).HasColumnName("StartsOn").IsRequired();
            builder.Property(d => d.EndsOn).HasColumnName("EndsOn");
            builder.Property(d => d.IsActive).HasColumnName("IsActive").IsRequired();
        }
    }
}
