using Microsoft.EntityFrameworkCore;
using Storage.Infra.Context;
using Storage.Infra.Migrations;
using Users.Infra.Storage.Configurations;

namespace Users.Infra.Storage
{
    public class UsersDbContext : CloudControlDbContext<UsersDbContext>
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options)
            : base(options, StorageSchemas.Shared)
        { }

        protected override void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SimpleUsersConfiguration());
        }

        public class UsersMigrationDefinition : CloudControlDbContextMigrationDefinition<UsersDbContext>
        {
            public override string SchemaName => StorageSchemas.Shared.Value;
            public override int Order => 1;
        }
    }
}
