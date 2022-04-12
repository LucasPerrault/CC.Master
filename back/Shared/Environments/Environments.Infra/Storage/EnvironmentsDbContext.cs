using Distributors.Infra.Storage.Configurations;
using Environments.Infra.Storage.Configurations;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Context;
using Storage.Infra.Migrations;

namespace Environments.Infra.Storage
{
    public class EnvironmentsDbContext : CloudControlDbContext<EnvironmentsDbContext>
    {
        public EnvironmentsDbContext(DbContextOptions<EnvironmentsDbContext> options)
            : base(options, StorageSchemas.Shared)
        { }

        protected override void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EnvironmentsConfiguration());
            modelBuilder.ApplyConfiguration(new EnvironmentLogsConfiguration());
            modelBuilder.ApplyConfiguration(new EnvironmentLogMessagesConfiguration());
            modelBuilder.ApplyConfiguration(new DistributorsConfiguration());
            modelBuilder.ApplyConfiguration(new EnvironmentAccessesConfiguration());
            modelBuilder.ApplyConfiguration(new EnvironmentSharedAccessesConfiguration());
            modelBuilder.ApplyConfiguration(new EnvironmentRenamingConfiguration());
        }

        public class EnvironmentsMigrationDefinition : CloudControlDbContextMigrationDefinition<EnvironmentsDbContext>
        {
            public override string SchemaName => StorageSchemas.Shared.Value;
            public override int Order => 1;
        }
    }
}
