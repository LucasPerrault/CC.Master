using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Contracts.Infra.Migrations
{
    public partial class AlterCountsAddContractsIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                $@"
                ALTER VIEW [billing].[Counts] WITH SCHEMABINDING AS
                    SELECT
                        c.idCount Id,
                        c.idContract ContractId,
                        c.idCommercialOffer CommercialOfferId,
                        c.countPeriod CountPeriod
                FROM [dbo].[Counts] c
                    LEFT JOIN [dbo].[Contracts] co ON co.idContract = c.idContract
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                ALTER VIEW [billing].[Counts] WITH SCHEMABINDING AS
                    SELECT
                        c.idCount Id,
                        c.idContract ContractId,
                        c.idCommercialOffer CommercialOfferId,
                        c.countPeriod CountPeriod
                    FROM [dbo].[Counts] c
            ");
        }
    }
}
