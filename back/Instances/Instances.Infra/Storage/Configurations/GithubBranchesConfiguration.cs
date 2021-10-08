using Instances.Domain.Github.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instances.Infra.Storage.Configurations
{
    public class GithubBranchesConfiguration : IEntityTypeConfiguration<GithubBranch>
    {
        public void Configure(EntityTypeBuilder<GithubBranch> builder)
        {
            builder.ToTable("GithubBranches");
            builder.HasKey(b => b.Id);
            builder.Property(p => p.Name).HasColumnName("name");
            builder.Property(p => p.IsDeleted).HasColumnName("idDeleted");
            builder.Property(p => p.CreatedAt).HasColumnName("createdAt");
            builder.Property(p => p.LastPushedAt).HasColumnName("lastPushedAt");
            builder.Property(p => p.DeletedAt).HasColumnName("deletedAt");
            builder.Property(p => p.HeadCommitHash).HasColumnName("headCommitHash");
            builder.Property(p => p.HeadCommitMessage).HasColumnName("headCommitMessage");
        }
    }
}
