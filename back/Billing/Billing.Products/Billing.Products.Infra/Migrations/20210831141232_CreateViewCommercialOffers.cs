using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Products.Infra.Migrations
{
    public partial class CreateViewCommercialOffers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql($@"
                CREATE VIEW [billing].[CommercialOffers] WITH SCHEMABINDING AS
                    SELECT
                    p.[idCommercialOffer] id,
                    p.[name] name,
                    p.[productId] productId
                FROM [dbo].[CommercialOffers] p
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommercialOffers",
                schema: "billing");

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                schema: "billing",
                table: "Solutions",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
