using Lucca.Core.AspNetCore.EfMigration.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Contracts.Infra.Migrations
{
    public partial class AlterPriceRowsUseDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropView("dbo", "PriceRows");
            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                schema: "billing",
                table: "PriceRows",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "FixedPrice",
                schema: "billing",
                table: "PriceRows",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.Sql(@"
                CREATE VIEW dbo.PriceRows WITH SCHEMABINDING as
                    select id
                        , listId
                        , maxIncludedCount
                        , CAST(unitPrice as float) unitPrice
                        , CAST(fixedPrice as float) fixedPrice
                    from billing.PriceRows;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropView("dbo", "PriceRows");
            migrationBuilder.AlterColumn<double>(
                name: "UnitPrice",
                schema: "billing",
                table: "PriceRows",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "FixedPrice",
                schema: "billing",
                table: "PriceRows",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");



            migrationBuilder.Sql(@"
                CREATE VIEW dbo.PriceRows WITH SCHEMABINDING as
                    select id
                        , listId
                        , maxIncludedCount
                        , unitPrice
                        , fixedPrice
                    from billing.PriceRows;
            ");
        }
    }
}
