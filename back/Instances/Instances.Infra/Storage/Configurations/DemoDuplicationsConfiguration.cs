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
                .HasMaxLength(SubdomainValidator.SubdomainMaxLength);

            builder.Property(d => d.SourceDemoSubdomain)
                .HasColumnName("sourceDemoSubdomain")
                .HasMaxLength(SubdomainValidator.SubdomainMaxLength);

            builder.Property(d => d.Password)
                .HasColumnName("password")
                .HasMaxLength(UsersPasswordHelper.MaxLength);

            builder.Property(d => d.CreatedAt).HasColumnName("createdAt");
            builder.Property(d => d.Progress).HasColumnName("progress");
            builder.Property(d => d.Comment).HasColumnName("comment");
            builder.Property(d => d.AuthorId).HasColumnName("authorId");
            builder.Property(d => d.DistributorId).HasColumnName("distributorId");

            builder.HasOne(d => d.Distributor).WithMany().HasForeignKey(d => d.DistributorId);
        }
    }
}
