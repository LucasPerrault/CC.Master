using Microsoft.EntityFrameworkCore.Migrations;

namespace Environments.Infra.Migrations
{
    public partial class AddIndexEnvironmentAccessesEnvironmentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentAccesses_EnvironmentId",
                schema: "dbo",
                table: "EnvironmentAccesses",
                column: "EnvironmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EnvironmentAccesses_EnvironmentId",
                schema: "dbo",
                table: "EnvironmentAccesses");
        }
    }
}
