using Microsoft.EntityFrameworkCore.Migrations;

namespace Environments.Infra.Migrations
{
    public partial class CreateEnvAccesses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE VIEW [shared].[EnvironmentAccesses] WITH SCHEMABINDING AS
                    SELECT 
                    a.id id,
                    a.distributorId distributorId,
                    a.environmentId environmentId,
                    a.authorId authorId,
                    a.type type,
                    a.startsAt startsAt,
                    a.endsAt endsAt,
                    a.revokedAt revokedAt,
                    a.comment comment,
                    a.revocationComment revocationComment,
                    a.lifecycle lifecycle
                FROM [dbo].[EnvironmentAccesses] a
            ");

            migrationBuilder.Sql($@"
                CREATE VIEW [shared].[EnvironmentSharedAccesses] WITH SCHEMABINDING AS
                    SELECT 
                    a.id id,
                    a.consumerId consumerId,
                    a.environmentId environmentId
                FROM [dbo].[EnvironmentSharedAccesses] a
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
