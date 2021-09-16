using Billing.Contracts.Infra.Storage.Configurations;
using Billing.Products.Infra.Storage.Configurations;
using Distributors.Infra.Storage.Configurations;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Context;
using Storage.Infra.Migrations;

namespace Billing.Contracts.Infra.Storage
{
    public class ContractsDbContext : CloudControlDbContext<ContractsDbContext>
    {
        public ContractsDbContext(DbContextOptions<ContractsDbContext> options)
            : base(options, StorageSchemas.Billing)
        { }

        protected override void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.ApplyConfiguration(new ContractConfiguration());
            modelBuilder.ApplyConfiguration(new ContractCommentConfiguration());
            modelBuilder.ApplyConfiguration(new EstablishmentAttachmentConfiguration());

            modelBuilder.ApplyConfiguration(new DistributorsConfiguration());
            modelBuilder.ApplyConfiguration(new CommercialOfferConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductSolutionsConfiguration());
            modelBuilder.ApplyConfiguration(new SolutionConfiguration());
        }
    }

    public class ContractsMigrationDefinition : CloudControlDbContextMigrationDefinition<ContractsDbContext>
    {
        public override string SchemaName => StorageSchemas.Billing.Value;
        public override int Order => 2;
    }
}
