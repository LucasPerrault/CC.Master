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
            builder.HasKey(cs => cs.Id);
            builder.Property(cs => cs.Code).HasColumnName("Code");
            builder.Property(cs => cs.Name).HasColumnName("Name");
            builder.Property(cs => cs.JenkinsProjectName).HasColumnName("JenkinsProjectName");
            builder.Property(cs => cs.JenkinsProjectUrl).HasColumnName("JenkinsProjectUrl");
            builder.Property(cs => cs.Type).HasColumnName("Type");
            builder.Property(cs => cs.Lifecycle).HasColumnName("Lifecycle");
            builder.HasOne(cs => cs.Config).WithOne().HasForeignKey<CodeSourceConfig>(c => c.CodeSourceId);
            builder.HasMany(cs => cs.ProductionVersions).WithOne(c => c.CodeSource).HasForeignKey(pv => pv.CodeSourceId);
            builder.HasMany(cs => cs.CodeSourceArtifacts).WithOne().HasForeignKey(csa => csa.CodeSourceId);

            builder
                .HasOne(cs => cs.Repo)
                .WithMany(r => r.CodeSources)
                .HasForeignKey(cs => cs.RepoId);

            builder.Ignore(cs => cs.CurrentProductionVersion);
        }
    }
    public class CodeSourceArtifactsConfiguration : IEntityTypeConfiguration<CodeSourceArtifacts>
    {
        public void Configure(EntityTypeBuilder<CodeSourceArtifacts> builder)
        {
            builder.ToTable("CodeSourceArtifacts");
            builder.HasKey(csa => csa.Id);
            builder.Property(csa => csa.CodeSourceId).HasColumnName("CodeSourceId");
            builder.Property(csa => csa.FileName).HasColumnName("FileName").HasMaxLength(255);
            builder.Property(csa => csa.ArtifactUrl).HasColumnName("ArtifactUrl").HasMaxLength(255);
            builder.Property(csa => csa.ArtifactType).HasColumnName("ArtifactType");
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
