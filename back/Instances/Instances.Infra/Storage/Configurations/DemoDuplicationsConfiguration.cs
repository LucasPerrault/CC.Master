using Instances.Domain.Demos;
using Instances.Infra.Demos;
using Instances.Infra.Instances.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instances.Infra.Storage.Configurations
{
    public class DemoDuplicationsConfiguration : IEntityTypeConfiguration<DemoDuplication>
    {
        public void Configure(EntityTypeBuilder<DemoDuplication> builder)
        {
            builder.ToTable("DemoDuplications");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Subdomain)
                .HasColumnName("subdomain")
                .HasMaxLength(SubdomainValidator.SubdomainMaxLength)
                .IsRequired();

            builder.Property(d => d.SourceDemoSubdomain)
                .HasColumnName("sourceDemoSubdomain")
                .HasMaxLength(SubdomainValidator.SubdomainMaxLength)
                .IsRequired();

            builder.Property(d => d.Password)
                .HasColumnName("password")
                .HasMaxLength(UsersPasswordHelper.MaxLength)
                .IsRequired();

            builder.Property(d => d.CreatedAt).HasColumnName("createdAt");
            builder.Property(d => d.ExternalId)
                .HasColumnName("externalId")
                .IsRequired();

            builder.Property(d => d.Progress)
                .HasColumnName("progress")
                .IsRequired();

            builder.Property(d => d.Comment)
                .HasColumnName("comment");

            builder.Property(d => d.AuthorId)
                .HasColumnName("authorId")
                .IsRequired();

            builder.Property(d => d.DistributorId)
                .HasColumnName("distributorId")
                .IsRequired();

            builder.HasOne(d => d.Distributor).WithMany().HasForeignKey(d => d.DistributorId);
        }
    }
}
