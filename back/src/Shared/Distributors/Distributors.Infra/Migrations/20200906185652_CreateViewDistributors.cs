using Microsoft.EntityFrameworkCore.Migrations;

namespace Distributors.Infra.Migrations
{
    public partial class CreateViewDistributors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE VIEW [shared].[Distributors] WITH SCHEMABINDING AS
                    SELECT 
                    d.idSalesforce id,
                    d.distributorName name,
                    d.distributorCode code
                FROM [dbo].[Distributors] d
                WHERE d.isActive = 1
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                DROP VIEW [shared].[Distributors]
            ");
        }
    }
}
