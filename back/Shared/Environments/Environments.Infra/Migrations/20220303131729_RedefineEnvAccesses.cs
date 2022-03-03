using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Environments.Infra.Migrations
{
    public partial class RedefineEnvAccesses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [shared].[EnvironmentSharedAccesses]");
            migrationBuilder.Sql("DROP VIEW [dbo].[EnvironmentSharedAccesses]");

            migrationBuilder.Sql(@"
            CREATE OR ALTER VIEW [shared].[EnvironmentSharedAccesses] WITH SCHEMABINDING AS
                WITH
                    relationtype as ( SELECT distinct(type) n FROM dbo.EnvironmentAccesses )
                    ,selfProvidingRelations as (
                        SELECT id providerId
                            , id consumerId
                            , 1 isActive
                            , relationtype.n type
                        FROM dbo.Distributors
                        LEFT JOIN relationtype ON 1 = 1
                    )
                    ,allRelations AS (
                        SELECT providerId, consumerId, isActive, type
                        FROM selfProvidingRelations
                        UNION
                        SELECT providerId, consumerId, isActive, type
                        FROM dbo.DistributorRelations
                    )
                SELECT id, environmentId, r.consumerId
                FROM dbo.EnvironmentAccesses a
                INNER JOIN allRelations r
                        ON r.providerId = distributorId
                        AND r.isActive = 1
                        AND r.type = a.type
                WHERE a.lifecycle = 1;
            ");

            migrationBuilder.Sql(@"
            CREATE OR ALTER VIEW [dbo].[EnvironmentSharedAccesses] WITH SCHEMABINDING AS
                SELECT
                    a.id id,
                    a.consumerId consumerId,
                    a.environmentId environmentId
                FROM [shared].[EnvironmentSharedAccesses] a
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
