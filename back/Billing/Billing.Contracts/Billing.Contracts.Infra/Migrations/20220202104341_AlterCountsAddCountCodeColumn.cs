using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Billing.Contracts.Infra.Migrations
{
    public partial class AlterCountsAddCountCodeColumn : Migration
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
                        c.countPeriod CountPeriod,
                        CAST(CASE WHEN c.Code = 'COUNT' THEN 0 ELSE 1 END AS int) AS Code
                FROM [dbo].[Counts] c
                    LEFT JOIN [dbo].[Contracts] co ON co.idContract = c.idContract
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
