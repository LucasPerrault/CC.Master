using Billing.Products.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Products.Infra.Storage.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).HasColumnName("Name");
            builder.Property(p => p.FamilyId).HasColumnName("FamilyId");

            builder.HasOne(p => p.Family).WithMany().HasForeignKey(p => p.FamilyId);
        }
    }

    public class ProductSolutionsConfiguration : IEntityTypeConfiguration<ProductSolution>
    {
        public void Configure(EntityTypeBuilder<ProductSolution> builder)
        {
            builder.ToTable("ProductSolutions");
            builder.HasKey(ps => new { ps.ProductId, ps.SolutionId });
            builder.HasOne(ps => ps.Product).WithMany(p => p.ProductSolutions).HasForeignKey(ps => ps.ProductId);
            builder.HasOne(ps => ps.Solution).WithMany(s => s.ProductSolutions).HasForeignKey(ps => ps.SolutionId);
        }
    }

    public class SolutionConfiguration : IEntityTypeConfiguration<Solution>
    {
        public void Configure(EntityTypeBuilder<Solution> builder)
        {
            builder.ToTable("Solutions");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).HasColumnName("Name");
        }
    }

    public class ProductFamilyConfiguration : IEntityTypeConfiguration<ProductFamily>
    {
        public void Configure(EntityTypeBuilder<ProductFamily> builder)
        {
            builder.ToTable("ProductFamilies");
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Name).HasColumnName("Name");
        }
    }
}
