using Instances.Domain.CodeSources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instances.Infra.Storage.Configurations
{
    public class CodeSourcesConfiguration : IEntityTypeConfiguration<CodeSource>
    {
        public void Configure(EntityTypeBuilder<CodeSource> builder)
        {
            builder.ToTable("CodeSources");
            builder.Property(cs => cs.Code).HasColumnName("Code");
            builder.Property(cs => cs.Name).HasColumnName("Name");
            builder.Property(cs => cs.JenkinsProjectName).HasColumnName("JenkinsProjectName");
            builder.Property(cs => cs.Type).HasColumnName("Type");
            builder.Property(cs => cs.GithubRepo).HasColumnName("GithubRepo");
            builder.Property(cs => cs.Lifecycle).HasColumnName("Lifecycle");

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
            builder.HasOne(c => c.CodeSource).WithOne(cs => cs.Config).HasForeignKey<CodeSourceConfig>(c => c.CodeSourceId);

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
