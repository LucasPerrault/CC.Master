using Billing.Contracts.Infra.Storage.Configurations;
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
        }
    }

    public class ContractsMigrationDefinition : CloudControlDbContextMigrationDefinition<ContractsDbContext>
    {
        public override string SchemaName => StorageSchemas.Billing.Value;
        public override int Order => 2;
    }
}
