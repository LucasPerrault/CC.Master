using Billing.Cmrr.Infra.Storage.Configurations;
using Distributors.Infra.Storage.Configurations;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Context;
using Storage.Infra.Migrations;

namespace Billing.Cmrr.Infra.Storage
{
    public class CmrrDbContext : CloudControlDbContext<CmrrDbContext>
    {
        public CmrrDbContext(DbContextOptions<CmrrDbContext> options)
            : base(options, StorageSchemas.Billing)
        {
        }

        protected override void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CmrrCountConfiguration());
            modelBuilder.ApplyConfiguration(new CmrrContractConfiguration());
            modelBuilder.ApplyConfiguration(new DistributorsConfiguration());
        }
    }

    public class CmrrMigrationDefinition : CloudControlDbContextMigrationDefinition<CmrrDbContext>
    {
        public override string SchemaName => StorageSchemas.Billing.Value;
        public override int Order => 3;
    }
}
