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
                        WHEN 'Figgo' THEN 38
                        WHEN 'Cleemy' THEN 26
                        WHEN 'Pagga' THEN 7
                        WHEN 'Pay Manager' THEN 1
                        WHEN 'Poplee Core RH' THEN 36
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
                        WHEN 'Timmi Timesheet' THEN 38
                        WHEN 'Anytime' THEN 5
                        WHEN 'Budget Insight' THEN 5
                        WHEN 'Timmi Project' THEN 20
                        WHEN 'Poplee REM' THEN 19
                        WHEN 'Dématérialisation' THEN 0
                        WHEN 'Synchronisation Figgo/GXP' THEN 5
                        WHEN 'Poplee Entretiens & Objectifs' THEN 39
                        WHEN 'Signature Electronique' THEN 0
                        WHEN 'Bloom At Work' THEN 20
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
