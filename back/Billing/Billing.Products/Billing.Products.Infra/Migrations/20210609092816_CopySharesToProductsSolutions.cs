using Microsoft.EntityFrameworkCore.Migrations;

namespace Billing.Products.Infra.Migrations
{
    public partial class CopySharesToProductsSolutions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "share",
                schema: "billing",
                table: "ProductsSolutions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE [CLOUDCONTROL].[billing].[ProductsSolutions]
                SET share = (SELECT defaultBreakdownShare from [CLOUDCONTROL].[billing].[Solutions] s where s.id = SolutionId)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "share",
                schema: "billing",
                table: "ProductsSolutions");
        }
    }
}
