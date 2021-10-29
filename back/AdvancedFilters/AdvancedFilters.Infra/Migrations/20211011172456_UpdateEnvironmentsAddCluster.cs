using Microsoft.EntityFrameworkCore.Migrations;

namespace AdvancedFilters.Infra.Migrations
{
    public partial class UpdateEnvironmentsAddCluster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cluster",
                schema: "cafe",
                table: "Environments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cluster",
                schema: "cafe",
                table: "Environments");
        }
    }
}
