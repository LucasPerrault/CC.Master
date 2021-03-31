using Lucca.Core.AspNetCore.EfMigration;
using Storage.Infra.Context;

namespace Storage.Infra.Migrations
{
    public abstract class CloudControlDbContextMigrationDefinition<T>
        : DbContextMigrationDefinition<T, CloudControlMigrationStartup> where T : CloudControlDbContext<T>
    {
        public override string MigrationsHistoryTableName => CloudControlMigrationStartup.MigrationsHistoryTableName;
    }
}
