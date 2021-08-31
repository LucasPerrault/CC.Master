using Billing.Products.Infra.Storage.Configurations;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Context;
using Storage.Infra.Migrations;

namespace Billing.Products.Infra.Storage
{
    public class ProductDbContext : CloudControlDbContext<ProductDbContext>
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options, StorageSchemas.Billing)
        {
        }

        protected override void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductSolutionsConfiguration());
            modelBuilder.ApplyConfiguration(new SolutionConfiguration());
            modelBuilder.ApplyConfiguration(new ProductFamilyConfiguration());
            modelBuilder.ApplyConfiguration(new CommercialOfferConfiguration());
        }
    }

    public class CmrrMigrationDefinition : CloudControlDbContextMigrationDefinition<ProductDbContext>
    {
        public override string SchemaName => StorageSchemas.Billing.Value;
        public override int Order => 3;
    }
}
