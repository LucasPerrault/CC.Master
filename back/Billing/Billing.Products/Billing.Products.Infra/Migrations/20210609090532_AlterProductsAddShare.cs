using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Products.Infra.Migrations
{
    public partial class AlterProductsAddShare : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "defaultBreakdownShare",
                schema: "billing",
                table: "Solutions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE [CLOUDCONTROL].[billing].[Solutions]
                SET defaultBreakdownShare = (
                CASE [Solutions].name
                       WHEN 'Figgo' THEN 100
                       WHEN 'Cleemy' THEN 100
                       WHEN 'Pagga' THEN 100
                       WHEN 'Pay Manager' THEN 1
                       WHEN 'Poplee Core RH' THEN 100
                       WHEN 'Portail' THEN 1
                       WHEN 'Spensy' THEN 1
                       WHEN 'Urba' THEN 1
                       WHEN 'Timesheet (old)' THEN 1
                       WHEN 'Ucal' THEN 1
                       WHEN 'Planner' THEN 1
                       WHEN 'Calendar' THEN 1
                       WHEN 'Autres' THEN 1
                       WHEN 'Admin' THEN 1
                       WHEN 'Project Manager' THEN 1
                       WHEN 'Timmi Timesheet' THEN 100
                       WHEN 'Anytime' THEN 10
                       WHEN 'Budget Insight' THEN 10
                       WHEN 'Timmi Project' THEN 100
                       WHEN 'Poplee REM' THEN 100
                       WHEN 'Dématérialisation' THEN 10
                       WHEN 'Synchronisation Figgo/GXP' THEN 10
                       WHEN 'Poplee Entretiens & Objectifs' THEN 100
                       WHEN 'Signature Electronique' THEN 10
                END)");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "defaultBreakdownShare",
                schema: "billing",
                table: "Solutions");
        }
    }
}
