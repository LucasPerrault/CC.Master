using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Cmrr.Infra.Migrations
{
    public partial class CreateTableCmrrCounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE VIEW [billing].[CmrrCounts] WITH SCHEMABINDING AS
                    SELECT 
                    c. idCount id,
                    c.idContract contractId,
                    c.countPeriod countPeriod,
                    c.billingStrategy billingStrategy,
                    c.accountingNumber accountingNumber,
                    c.entryNumber entryNumber,
                    c.code code,
                    a.currencyId currencyId,
                    a.currencyAmount currencyAmount,
                    a.euroAmount euroAmount,
                    c.discount1 luccaDiscount,
                    c.discount2 distributorDiscount,
                    c.countDate createdAt
                    FROM [dbo].[Counts] c
                    INNER JOIN [dbo].[Accountings] a on a.entryNumber = c.entryNumber
                    WHERE accountNumber = '401'
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
