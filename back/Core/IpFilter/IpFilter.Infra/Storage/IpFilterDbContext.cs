using IpFilter.Infra.Storage.Configurations;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Context;
using Storage.Infra.Migrations;

namespace IpFilter.Infra.Storage
{
    public class IpFilterDbContext: CloudControlDbContext<IpFilterDbContext>
    {
        public IpFilterDbContext(DbContextOptions<IpFilterDbContext> options)
            : base(options, StorageSchemas.Shared)
        { }

        protected override void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new IpFilterAuthorizationConfiguration());
        }
    }

    public class IpFilterMigrationDefinition : CloudControlDbContextMigrationDefinition<IpFilterDbContext>
    {
        public override string SchemaName => StorageSchemas.Shared.Value;
        public override int Order => 2;
    }
}
