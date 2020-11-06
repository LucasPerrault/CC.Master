using Storage.Infra.Context;
using Storage.Infra.Migrations;

namespace IpFilter.Infra.Storage
{
    public class IpFilterMigrationDefinition : CloudControlDbContextMigrationDefinition<IpFilterDbContext>
    {
        public override string SchemaName => StorageSchemas.Shared.Value;
        public override int Order => 2;
    }
}
