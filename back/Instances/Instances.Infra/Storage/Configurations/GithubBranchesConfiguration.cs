using Instances.Domain.CodeSources;
using Instances.Domain.Github.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

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

            builder
                .HasMany(b => b.CodeSources)
                .WithMany(c => c.GithubBranches)
                .UsingEntity<Dictionary<string, object>>(
                    "GithubBranchesCodeSources",
                    b => b.HasOne<CodeSource>().WithMany().HasForeignKey("codeSourceId"),
                    c => c.HasOne<GithubBranch>().WithMany().HasForeignKey("githubBranchId")
                );
        }
    }
}
