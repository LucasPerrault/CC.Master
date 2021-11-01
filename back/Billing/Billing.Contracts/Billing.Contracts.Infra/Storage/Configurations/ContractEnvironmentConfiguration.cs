using Billing.Contracts.Domain.Environments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Contracts.Infra.Storage.Configurations
{
    public class ContractEnvironmentConfiguration : IEntityTypeConfiguration<ContractEnvironment>
    {
        public void Configure(EntityTypeBuilder<ContractEnvironment> builder)
        {
            builder.ToTable("ContractEnvironments");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Subdomain).HasColumnName("Subdomain");
            builder.HasMany(d => d.Establishments).WithOne().HasForeignKey(e => e.EnvironmentId);
        }
    }

    public class EstablishmentConfiguration : IEntityTypeConfiguration<Establishment>
    {
        public void Configure(EntityTypeBuilder<Establishment> builder)
        {
            builder.ToTable("Establishments");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).HasColumnName("Name");
            builder.Property(e => e.EnvironmentId).HasColumnName("EnvironmentId");
            builder.Property(e => e.RemoteId).HasColumnName("RemoteId");
            builder.Property(e => e.LegalUnitId).HasColumnName("LegalUnitId");
            builder.Property(e => e.IsActive).HasColumnName("IsActive");
            builder.HasMany(d => d.Exclusions).WithOne().HasForeignKey(e => e.EstablishmentId);
            builder.HasMany(d => d.Attachments).WithOne().HasForeignKey(a => a.EstablishmentId);
        }
    }

    public class EstablishmentAttachmentConfiguration : IEntityTypeConfiguration<EstablishmentAttachment>
    {
        public void Configure(EntityTypeBuilder<EstablishmentAttachment> builder)
        {
            builder.ToTable("EstablishmentAttachments");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.EstablishmentId).HasColumnName("EstablishmentId").IsRequired();
            builder.Property(d => d.EstablishmentName).HasColumnName("EstablishmentName").IsRequired();
            builder.Property(d => d.EstablishmentRemoteId).HasColumnName("EstablishmentRemoteId").IsRequired();
            builder.Property(d => d.StartsOn).HasColumnName("StartsOn").IsRequired();
            builder.Property(d => d.EndsOn).HasColumnName("EndsOn");

            builder.Property(d => d.ContractId).HasColumnName("ContractId").IsRequired();
            builder.Ignore(d => d.ProductId);

            builder.Property(d => d.EndReason).HasColumnName("EndReason");
            builder.Property(d => d.NumberOfFreeMonths).HasColumnName("NumberOfFreeMonths");
        }
    }

    public class EstablishmentExclusionConfiguration : IEntityTypeConfiguration<EstablishmentExclusion>
    {
        public void Configure(EntityTypeBuilder<EstablishmentExclusion> builder)
        {
            builder.ToTable("EstablishmentExclusions");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ProductId).HasColumnName("ProductId");
            builder.Property(e => e.AuthorId).HasColumnName("AuthorId");
            builder.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
            builder.Property(e => e.EstablishmentId).HasColumnName("EstablishmentId");
        }
    }
}
