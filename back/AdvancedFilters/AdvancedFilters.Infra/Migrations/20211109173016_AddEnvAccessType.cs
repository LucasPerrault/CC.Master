using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedFilters.Infra.Migrations
{
    public partial class AddEnvAccessType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Environments_Subdomain",
                schema: "cafe",
                table: "Environments");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "cafe",
                table: "EnvironmentAccesses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Environments_Subdomain",
                schema: "cafe",
                table: "Environments",
                column: "Subdomain");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Environments_Subdomain",
                schema: "cafe",
                table: "Environments");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "cafe",
                table: "EnvironmentAccesses");

            migrationBuilder.CreateIndex(
                name: "IX_Environments_Subdomain",
                schema: "cafe",
                table: "Environments",
                column: "Subdomain")
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
