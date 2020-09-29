using Microsoft.EntityFrameworkCore.Migrations;

namespace Distributors.Infra.Migrations
{
    public partial class CreateViewDistributorDomains : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE VIEW [shared].[DistributorDomains] WITH SCHEMABINDING AS
                    SELECT 
                    d.id id,
                    d.distributorId distributorId,
                    d.domain domain
                FROM [dbo].[DistributorsDomains] d
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                DROP VIEW [shared].[DistributorDomains]
            ");
        }
    }
}
