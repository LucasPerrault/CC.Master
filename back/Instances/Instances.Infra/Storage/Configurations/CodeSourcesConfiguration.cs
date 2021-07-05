using Instances.Domain.CodeSources;
using Instances.Infra.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instances.Infra.Storage.Configurations
{
    public class CodeSourcesConfiguration : IEntityTypeConfiguration<StoredCodeSource>
    {
        public void Configure(EntityTypeBuilder<StoredCodeSource> builder)
        {
            builder.ToTable("CodeSources");
            builder.HasKey(cs => cs.Id);
            builder.Property(cs => cs.Code).HasColumnName("Code");
            builder.Property(cs => cs.Name).HasColumnName("Name");
            builder.Property(cs => cs.JenkinsProjectName).HasColumnName("JenkinsProjectName");
            builder.Property(cs => cs.JenkinsProjectUrl).HasColumnName("JenkinsProjectUrl");
            builder.Property(cs => cs.Type).HasColumnName("Type");
            builder.Property(cs => cs.GithubRepo).HasColumnName("GithubRepo");
            builder.Property(cs => cs.Lifecycle).HasColumnName("Lifecycle");
            builder.HasOne(cs => cs.Config).WithOne().HasForeignKey<CodeSourceConfig>(c => c.CodeSourceId);
            builder.HasMany(cs => cs.ProductionVersions).WithOne().HasForeignKey(pv => pv.CodeSourceId);
        }
    }

    public class CodeSourceConfigsConfiguration : IEntityTypeConfiguration<CodeSourceConfig>
    {
        public void Configure(EntityTypeBuilder<CodeSourceConfig> builder)
        {
            builder.ToTable("CodeSourceConfigs");
            builder.HasKey("CodeSourceId");
            builder.Property(c => c.AppPath).HasColumnName("AppPath");
            builder.Property(c => c.Subdomain).HasColumnName("Subdomain");
            builder.Property(c => c.IisServerPath).HasColumnName("IisServerPath");
            builder.Property(c => c.IsPrivate).HasColumnName("IsPrivate");

        }
    }

    public class CodeSourceProductionVersionsConfiguration : IEntityTypeConfiguration<CodeSourceProductionVersion>
    {
        public void Configure(EntityTypeBuilder<CodeSourceProductionVersion> builder)
        {
            builder.ToTable("CodeSourcesProductionVersions");
            builder.HasKey("Id");
            builder.Property(c => c.CodeSourceId).HasColumnName("CodeSourceId");
            builder.Property(c => c.BranchName).HasColumnName("BranchName");
            builder.Property(c => c.JenkinsBuildNumber).HasColumnName("JenkinsBuildNumber");
            builder.Property(c => c.CommitHash).HasColumnName("CommitHash");
            builder.Property(c => c.Date).HasColumnName("Date");
        }
    }
}
