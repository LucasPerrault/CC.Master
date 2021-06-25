using Distributors.Infra.Storage.Configurations;
using Instances.Infra.Storage.Configurations;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Context;
using Storage.Infra.Migrations;
using Users.Infra.Storage.Configurations;

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
            modelBuilder.ApplyConfiguration(new InstanceDuplicationsConfiguration());

            modelBuilder.ApplyConfiguration(new DemosConfiguration());
            modelBuilder.ApplyConfiguration(new DemoDuplicationsConfiguration());

            modelBuilder.ApplyConfiguration(new CodeSourcesConfiguration());
            modelBuilder.ApplyConfiguration(new CodeSourceConfigsConfiguration());
            modelBuilder.ApplyConfiguration(new CodeSourceProductionVersionsConfiguration());

            // shared
            modelBuilder.ApplyConfiguration(new DistributorsConfiguration());
            modelBuilder.ApplyConfiguration(new SimpleUsersConfiguration());
        }

        public class InstancesMigrationDefinition : CloudControlDbContextMigrationDefinition<InstancesDbContext>
        {
            public override string SchemaName => StorageSchemas.Instances.Value;
            public override int Order => 2;
        }
    }
}
