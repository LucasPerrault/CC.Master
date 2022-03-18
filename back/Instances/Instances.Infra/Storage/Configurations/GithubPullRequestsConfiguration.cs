using Instances.Domain.CodeSources;
using Instances.Domain.Github.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Instances.Infra.Storage.Configurations
{
    public class GithubPullRequestsConfiguration : IEntityTypeConfiguration<GithubPullRequest>
    {
        public void Configure(EntityTypeBuilder<GithubPullRequest> builder)
        {
            builder.ToTable("GithubPullRequests");
            builder.HasKey(pr => pr.Id);
            builder.Property(pr => pr.Number).HasColumnName("number");
            builder.Property(pr => pr.IsOpened).HasColumnName("isOpened");
            builder.Property(pr => pr.OpenedAt).HasColumnName("openedAt");
            builder.Property(pr => pr.MergedAt).HasColumnName("mergedAt");
            builder.Property(pr => pr.ClosedAt).HasColumnName("closedAt");
            builder.Property(pr => pr.Title).HasColumnName("title");

            builder.Property(pr => pr.OriginBranchId).HasColumnName("originBranchId");
            builder.HasOne(pr => pr.OriginBranch).WithMany().HasForeignKey(pr => pr.OriginBranchId);

            builder
                .HasOne(pr => pr.Repo)
                .WithMany()
                .HasForeignKey(pr => pr.RepoId);
        }
    }
}
