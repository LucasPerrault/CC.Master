using Microsoft.EntityFrameworkCore.Migrations;

namespace Instances.Infra.Migrations
{
    public partial class MoveDemosToInstances : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                ALTER SCHEMA instances TRANSFER dbo.Demos;
            ");

            migrationBuilder.Sql($@"
                ALTER TABLE instances.Demos DROP COLUMN creationSource;
                ALTER TABLE instances.Demos DROP CONSTRAINT [DF__Demos__protected__22751F6C]
                ALTER TABLE instances.Demos DROP COLUMN Protected;
                EXEC sp_rename 'instances.Demos.dtCreation', 'createdAt', 'COLUMN';
            ");

            migrationBuilder.Sql($@"
                CREATE VIEW [dbo].[Demos] WITH SCHEMABINDING AS
                    SELECT
                    d.[id] id
                    ,d.[subdomain] subdomain
                    ,d.[createdAt] dtCreation
                    ,d.[isActive] isActive
                    ,d.[distributorId] distributorId
                    ,d.[instanceId] instanceId
                    ,d.[comment] comment
                    ,d.[deletionScheduledOn] deletionScheduledOn
                FROM [instances].[Demos] d
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                DROP VIEW [dbo].[Demos]
            ");
            migrationBuilder.Sql($@"
                ALTER SCHEMA dbo TRANSFER instances.Demos;  
            ");
        }
    }
}
