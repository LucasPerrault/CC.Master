using Instances.Domain.Demos;
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

            builder.Property(d => d.InstanceDuplicationId)
                .IsRequired();

            builder.Property(d => d.SourceDemoId)
                .HasColumnName("sourceDemoId")
                .IsRequired();

            builder.Property(d => d.Password)
                .HasColumnName("password")
                .HasMaxLength(UsersPasswordHelper.MaxLength)
                .IsRequired();

            builder.Property(d => d.CreatedAt)
                .HasColumnName("createdAt");

            builder.Property(d => d.Comment)
                .HasColumnName("comment");

            builder.Property(d => d.AuthorId)
                .HasColumnName("authorId")
                .IsRequired();

            builder.Ignore(d => d.DistributorId);

            builder.HasOne(d => d.InstanceDuplication).WithMany().HasForeignKey(d => d.InstanceDuplicationId);
        }
    }
}
