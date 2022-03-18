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
            builder.Property(b => b.Name).HasColumnName("name");
            builder.Property(b => b.IsDeleted).HasColumnName("isDeleted");
            builder.Property(b => b.CreatedAt).HasColumnName("createdAt");
            builder.Property(b => b.LastPushedAt).HasColumnName("lastPushedAt");
            builder.Property(b => b.DeletedAt).HasColumnName("deletedAt");
            builder.Property(b => b.HeadCommitHash).HasColumnName("headCommitHash");
            builder.Property(b => b.HeadCommitMessage).HasColumnName("headCommitMessage");
            builder.Property(b => b.HelmChart).HasColumnName("helmChart");

            builder
                .HasOne(b => b.Repo)
                .WithMany(r => r.GithubBranches)
                .HasForeignKey(b => b.RepoId);
        }
    }
}
