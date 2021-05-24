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
            builder.Property(p => p.Code).HasColumnName("Code");
            builder.Property(p => p.IsFreeUse).HasColumnName("IsFreeUse");
            builder.Property(p => p.FamilyId).HasColumnName("FamilyId");
            builder.Property(p => p.IsEligibleToMinimalBilling).HasColumnName("IsEligibleToMinimalBilling");
            builder.Property(p => p.IsMultiSuite).HasColumnName("IsMultiSuite");
            builder.Property(p => p.IsPromoted).HasColumnName("IsPromoted");
            builder.Property(p => p.ParentId).HasColumnName("ParentId");
            builder.Property(p => p.SalesForceCode).HasColumnName("SalesForceCode");
            builder.Property(p => p.ApplicationCode).HasColumnName("ApplicationCode");
            builder.Property(p => p.DeployRoute).HasColumnName("DeployRoute");

            builder.HasOne(p => p.Family).WithMany().HasForeignKey(p => p.FamilyId);
        }
    }

    public class ProductSolutionsConfiguration : IEntityTypeConfiguration<ProductSolution>
    {
        public void Configure(EntityTypeBuilder<ProductSolution> builder)
        {
            builder.ToTable("ProductsSolutions");
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
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).HasColumnName("Name");
            builder.Property(s => s.Code).HasColumnName("Code");
            builder.Property(s => s.ParentId).HasColumnName("ParentId");
            builder.Property(s => s.IsContactNeeded).HasColumnName("IsContactNeeded");

            builder.HasOne(s => s.BusinessUnit).WithMany(bu => bu.Solutions).HasForeignKey(s => s.BusinessUnitId);
        }
    }

    public class BusinessUnitConfiguration : IEntityTypeConfiguration<BusinessUnit>
    {
        public void Configure(EntityTypeBuilder<BusinessUnit> builder)
        {
            builder.ToTable("BusinessUnits");
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
