using Microsoft.EntityFrameworkCore.Migrations;

namespace IpFilter.Infra.Migrations
{
    public partial class IpFilterInitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE VIEW [shared].[IpFilterAuthorizations] WITH SCHEMABINDING AS
                    SELECT 
                    d.id id,
                    d.userId userId,
                    d.ip ipAddress,
                    d.device device,
                    d.createdAt createdAt,
                    d.expiresAt expiresAt
                FROM [dbo].[UserIpDevices] d
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                DROP VIEW [shared].[IpFilterAuthorizations]
            ");
        }
    }
}
