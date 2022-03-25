using Instances.Domain.Github.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instances.Infra.Storage.Configurations
{
    public class GithubReposConfiguration : IEntityTypeConfiguration<GithubRepo>
    {
        public void Configure(EntityTypeBuilder<GithubRepo> builder)
        {
            builder.ToTable("GithubRepos");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name).HasColumnName("Name");
            builder.Property(r => r.Url).HasColumnName("Url");
        }
    }
}
