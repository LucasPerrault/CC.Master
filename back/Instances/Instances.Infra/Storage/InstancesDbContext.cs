using Distributors.Infra.Storage.Configurations;
using Instances.Infra.Storage.Configurations;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Context;
using Storage.Infra.Migrations;

namespace Instances.Infra.Storage
{
    public class InstancesDbContext : CloudControlDbContext<InstancesDbContext>
    {
        public InstancesDbContext(DbContextOptions<InstancesDbContext> options)
            : base(options, StorageSchemas.Instances)
        { }

        protected override void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new InstancesConfiguration());
            modelBuilder.ApplyConfiguration(new DemosConfiguration());

            // shared
            modelBuilder.ApplyConfiguration(new DistributorsConfiguration());

        }

        public class InstancesMigrationDefinition : CloudControlDbContextMigrationDefinition<InstancesDbContext>
        {
            public override string SchemaName => StorageSchemas.Instances.Value;
            public override int Order => 1;
        }
    }
}
