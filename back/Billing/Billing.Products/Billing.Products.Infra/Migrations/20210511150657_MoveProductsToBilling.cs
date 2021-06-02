using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Products.Infra.Migrations
{
    public partial class MoveProductsToBilling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                ALTER SCHEMA billing TRANSFER dbo.Products;
            ");

            migrationBuilder.Sql($@"
                CREATE VIEW [dbo].[Products] WITH SCHEMABINDING AS
                    SELECT
                    p.[id] id,
                    p.[name] name,
                    p.[code] code,
                    p.applicationCode applicationCode,
                    p.[parentId] parentId,
                    p.isEligibleToMinimalBilling isEligibleToMinimalBilling,
                    p.isMultiSuite isMultiSuite,
                    p.isPromoted isPromoted,
                    p.salesforceCode salesforceCode,
                    p.isFreeUse isFreeUse,
                    p.deployRoute deployRoute
                FROM [billing].[Products] p
            ");


            migrationBuilder.Sql($@"
                ALTER SCHEMA billing TRANSFER dbo.Solutions;
            ");

            migrationBuilder.Sql($@"
                CREATE VIEW [dbo].[Solutions] WITH SCHEMABINDING AS
                    SELECT
                    s.[id] id,
                    s.[name] name,
                    s.[code] code,
                    s.[parentId] parentId,
                    s.[IsContactNeeded] isContactNeeded 
                FROM [billing].[Solutions] s
            ");


            migrationBuilder.Sql($@"
                ALTER SCHEMA billing TRANSFER dbo.[ProductsSolutions];
            ");

            migrationBuilder.Sql($@"
                CREATE VIEW [dbo].[ProductsSolutions] WITH SCHEMABINDING AS
                    SELECT
                    ps.[ProductId] productId,
                    ps.[SolutionId] solutionId
                FROM [billing].[ProductsSolutions] ps
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
