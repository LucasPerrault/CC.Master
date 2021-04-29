using Instances.Domain.Instances;
using Instances.Infra.Demos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instances.Infra.Storage.Configurations
{
    public class InstanceDuplicationsConfiguration : IEntityTypeConfiguration<InstanceDuplication>
    {
        public void Configure(EntityTypeBuilder<InstanceDuplication> builder)
        {
            builder.ToTable("InstanceDuplications");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.DistributorId)
                .HasColumnName("distributorId")
                .IsRequired();

            builder.Property(d => d.SourceSubdomain)
                .HasColumnName("sourceSubdomain")
                .HasMaxLength(SubdomainValidator.SubdomainMaxLength)
                .IsRequired();

            builder.Property(d => d.TargetSubdomain)
                .HasColumnName("targetSubdomain")
                .HasMaxLength(SubdomainValidator.SubdomainMaxLength)
                .IsRequired();

            builder.Property(d => d.SourceCluster)
                .HasColumnName("sourceCluster")
                .IsRequired();

            builder.Property(d => d.TargetCluster)
                .HasColumnName("targetCluster")
                .IsRequired();

            builder.Property(d => d.SourceType)
                .HasColumnName("sourceType")
                .IsRequired();

            builder.Property(d => d.TargetType)
                .HasColumnName("targetType")
                .IsRequired();

            builder.Property(d => d.Progress)
                .HasColumnName("progress")
                .IsRequired();

            builder.Property(d => d.StartedAt)
                .HasColumnName("startedAt")
                .IsRequired();

            builder.Property(d => d.EndedAt)
                .HasColumnName("endedAt");

            builder.HasOne(d => d.Distributor)
                .WithMany()
                .HasForeignKey(d => d.DistributorId);
        }
    }
}
