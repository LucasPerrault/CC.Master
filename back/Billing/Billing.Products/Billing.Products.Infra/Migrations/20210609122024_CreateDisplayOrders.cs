using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Products.Infra.Migrations
{
    public partial class CreateDisplayOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                schema: "billing",
                table: "ProductFamilies",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE [CLOUDCONTROL].[billing].[ProductFamilies]
                SET DisplayOrder = (
                CASE [ProductFamilies].name
                       WHEN 'Figgo' THEN 1
                       WHEN 'Timmi' THEN 2
                       WHEN 'Cleemy' THEN 3
                       WHEN 'Poplee' THEN 4
                       WHEN 'Pagga' THEN 5
                       WHEN 'Bloom' THEN 6
                       WHEN 'Essentiel SIRH' THEN 7
                       WHEN 'Lucca pour la Paie' THEN 8
                       WHEN 'Startup' THEN 9
                       WHEN 'Gratuits' THEN 10
                       WHEN 'Autres' THEN 11
                END)");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                schema: "billing",
                table: "BusinessUnit",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE [CLOUDCONTROL].[billing].[BusinessUnit]
                SET DisplayOrder = (
                CASE [BusinessUnit].name
                       WHEN 'GTA' THEN 1
                       WHEN 'Finance' THEN 2
                       WHEN 'Talent' THEN 3
                       WHEN 'Pay' THEN 4
                       WHEN 'Core' THEN 5
                       WHEN 'Engagement' THEN 6
                       WHEN 'Autres' THEN 7
                END)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                schema: "billing",
                table: "ProductFamilies");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                schema: "billing",
                table: "BusinessUnit");
        }
    }
}
