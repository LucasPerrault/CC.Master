using Storage.Infra.Context;
using Storage.Infra.Migrations;

namespace Distributors.Infra.Storage
{
    public class DistributorsMigrationDefinition : CloudControlDbContextMigrationDefinition<DistributorsDbContext>
    {
        public override string SchemaName => StorageSchemas.Shared.Value;
        public override int Order => 1;
    }
}
