using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class AddCountsView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE VIEW [billing].[Counts] WITH SCHEMABINDING AS
                    SELECT 
                        c.idCount Id,
                        c.idContract ContractId,
                        c.idCommercialOffer CommercialOfferId
                    FROM [dbo].[Counts] c
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        { }
    }
}
