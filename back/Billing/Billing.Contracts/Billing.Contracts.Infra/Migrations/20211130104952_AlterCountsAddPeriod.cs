using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class AlterCountsAddPeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                ALTER VIEW [billing].[Counts] WITH SCHEMABINDING AS
                    SELECT
                        c.idCount Id,
                        c.idContract ContractId,
                        c.idCommercialOffer CommercialOfferId
                    FROM [dbo].[Counts] c
");
        }
    }
}
