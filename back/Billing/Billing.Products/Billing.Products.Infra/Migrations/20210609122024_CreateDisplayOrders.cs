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
                       WHEN 'Timmi Timesheet' THEN 2
                       WHEN 'Timmi Project' THEN 3
                       WHEN 'Cleemy Expenses' THEN 4
                       WHEN 'Poplee Entretiens & Objectifs' THEN 5
                       WHEN 'Pagga' THEN 6
                       WHEN 'Poplee REM' THEN 7
                       WHEN 'Poplee Core RH' THEN 8
                       WHEN 'Bloom' THEN 9
                       WHEN 'Essentiel' THEN 10
                       WHEN 'SIRH et Paie' THEN 11
                       WHEN 'Suite Startup' THEN 12
                       WHEN 'Gratuits' THEN 13
                       WHEN 'Autres' THEN 14
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
